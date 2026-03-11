#include "FileTrack.h"

using namespace juce;
using namespace std;

_FileTrack::_FileTrack(std::string filePath) {
    formatManager.registerBasicFormats();

    File audioFile(juce::String::fromUTF8(filePath.c_str()));

    if (audioFile.existsAsFile()) {
        readerSource.reset(formatManager.createReaderFor(audioFile));
        changeGain(0.5f);
    }

    visSampleArrayHEAP = (float*)malloc(sizeof(float[512]));
    visSampleArraySTACK = new (visSampleArrayHEAP) float[512]();
}

_FileTrack::~_FileTrack() {
    int count = 0;

    while (count < DSPEffectChainLength) {
        removeDSPEffect(DSPEffectChain[count]);
        count++;
    }
}

#pragma region Juce_Overriden_methods

void _FileTrack::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    outputGain->prepare(spec);
    outputGain->setGainLinear(gain);
    outputGain->setRampDurationSeconds(0.02); // super important

    tempBuffer.setSize(spec.numChannels, readerSource->lengthInSamples, false, false, true);

    readerSource->read(&tempBuffer,
        0,
        readerSource->lengthInSamples,
        currentSampleIndex,
        true,
        false);

    int options =
        RubberBand::RubberBandStretcher::OptionProcessRealTime |
        RubberBand::RubberBandStretcher::OptionThreadingNever |
        RubberBand::RubberBandStretcher::OptionPitchHighQuality;

    rbbStretcher = std::make_unique<RubberBand::RubberBandStretcher>(spec.sampleRate, numChannels, options);
    changeFileTempo(setTempoRatio);
}

void _FileTrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray = true)
{
    bufferToFill.clearActiveBufferRegion();

    if (currentSampleIndex < tempBuffer.getNumSamples()) {
        const int numChannels = bufferToFill.buffer->getNumChannels();
        const int requestedSamples = bufferToFill.numSamples;

        int availableInput = min(requestedSamples, tempBuffer.getNumSamples() - currentSampleIndex);

        std::vector<const float*> input(numChannels);

        while (rbbStretcher->available() < requestedSamples)
        {
            if (currentSampleIndex >= tempBuffer.getNumSamples())
                break;

            int blockToFeed = juce::jmin(
                (int)spec.maximumBlockSize,
                tempBuffer.getNumSamples() - currentSampleIndex
            );

            for (int ch = 0; ch < numChannels; ++ch)
                input[ch] = tempBuffer.getReadPointer(ch, currentSampleIndex);

            rbbStretcher->process(input.data(), blockToFeed, false);

            currentSampleIndex += blockToFeed;
        }

        int availableOutput = rbbStretcher->available();

        if (availableOutput > 0)
        {
            int samplesToRetrieve = min(requestedSamples, availableOutput);

            std::vector<float*> output(numChannels);

            for (int ch = 0; ch < numChannels; ++ch)
                output[ch] = bufferToFill.buffer->getWritePointer(ch, bufferToFill.startSample);

            rbbStretcher->retrieve(output.data(), samplesToRetrieve);
        }

        juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
        juce::dsp::ProcessContextReplacing<float> context(audioBlock);

        for (int i = 0; i < DSPEffectChainLength; i++) {
            DSPEffectChain[i]->process(context);
        }

        outputGain->process(context);

        if (fillVisualizationArray) {
            for (int i = 0; i < spec.maximumBlockSize; i++) {
                try {
                    visSampleArraySTACK[i] = context.getOutputBlock().getChannelPointer(0)[i];
                }
                catch (error_code a) {
                    break;
                }
            }
        }
    }
    else if (isRepeating == true) {
        currentSampleIndex = 0;
        currentSampleContinuousPosition = 0;
    }
}

void _FileTrack::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region CustomMethods

void _FileTrack::changeRepeatingMode(bool newRepeatState) {
    isRepeating = newRepeatState;
}

void _FileTrack::changeFileTimePitchCouplingMode(bool newFileTimePitchCouplingMode) {
    timePitchCouplingMode = newFileTimePitchCouplingMode;

    if (timePitchCouplingMode) {
        internalTempoRatio = setPitchRatio;
        rbbStretcher->setTimeRatio(1 / internalTempoRatio);
    }
    else {
        internalTempoRatio = setTempoRatio;
        rbbStretcher->setTimeRatio(1 / internalTempoRatio);
    }
}

void _FileTrack::changeFileTempo(float newTempo) {
    setTempoRatio = newTempo;
    rbbStretcher->setTimeRatio(1 / setTempoRatio);
}

void _FileTrack::changeFilePitch(float newPitch) {
    setPitchRatio = newPitch;
    rbbStretcher->setPitchScale(setPitchRatio);
    if (timePitchCouplingMode) rbbStretcher->setTimeRatio(1 / setPitchRatio);
}

void _FileTrack::resetTime() {
    currentSampleIndex = 0;
    currentSampleContinuousPosition = 0;
}

char _FileTrack::getEngineType() {
    return 'F';
}

float _FileTrack::resampleSample(int channelIndex, float sampleIndex, float _pitchRatio)
{
    float resSample = tempBuffer.getSample(channelIndex, currentSampleIndex + sampleIndex);

    if (currentSampleContinuousPosition + sampleIndex < tempBuffer.getNumSamples() - 1) {
        int index = (int)currentSampleContinuousPosition;
        float frac = currentSampleContinuousPosition - index;

        float s1 = tempBuffer.getSample(channelIndex, index);
        float s2 = tempBuffer.getSample(channelIndex, index + 1);
        float interpolated = s1 + frac * (s2 - s1);

        resSample = interpolated;
    }

    currentSampleContinuousPosition += _pitchRatio;

    return resSample;
}

#pragma endregion