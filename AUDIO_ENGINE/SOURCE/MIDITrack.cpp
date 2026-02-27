#include "MIDITrack.h"

_MIDITrack::_MIDITrack() {
    //formatManager.registerBasicFormats();

    visSampleArrayHEAP = (float*)malloc(sizeof(float[512]));
    visSampleArraySTACK = new (visSampleArrayHEAP) float[512]();
}

_MIDITrack::~_MIDITrack() {
    int count = 0;

    while (count < DSPEffectChainLength) {
        removeDSPEffect(DSPEffectChain[count]);
        count++;
    }
}

#pragma region Juce_Overriden_methods

void _MIDITrack::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    outputGain->prepare(spec);
    outputGain->setGainLinear(gain);
    outputGain->setRampDurationSeconds(0.02); // super important

    //tempBuffer.setSize(spec.numChannels, readerSource->lengthInSamples, false, false, true);

    /*readerSource->read(&tempBuffer,
        0,
        readerSource->lengthInSamples,
        currentSampleIndex,
        true,
        false);*/

    int options =
        RubberBand::RubberBandStretcher::OptionProcessRealTime |
        RubberBand::RubberBandStretcher::OptionThreadingNever |
        RubberBand::RubberBandStretcher::OptionPitchHighQuality;

    rbbStretcher = std::make_unique<RubberBand::RubberBandStretcher>(spec.sampleRate, numChannels, options);
    changeFileTempo(setTempoRatio);
}

void _MIDITrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) {
    bufferToFill.clearActiveBufferRegion();

    if (currentSampleIndex < MIDITrackBuffer.getNumSamples()) {
        bufferToFill.clearActiveBufferRegion();
        for (int channel = 0; channel < bufferToFill.buffer->getNumChannels(); ++channel)
        {
            float* buffer = bufferToFill.buffer->getWritePointer(channel, bufferToFill.startSample);

            for (int sample = 0; sample < min(bufferToFill.numSamples, MIDITrackBuffer.getNumSamples() - currentSampleIndex); ++sample)
            {
                buffer[sample] = MIDITrackBuffer.getSample(channel, currentSampleIndex + sample);
            }
        }

        currentSampleIndex += bufferToFill.numSamples;

        juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
        juce::dsp::ProcessContextReplacing<float> context(audioBlock);

        outputGain->process(context);
    }
    else if (isRepeating == true) {
        currentSampleIndex = 0;
        currentSampleContinuousPosition = 0;
    }
}

void _MIDITrack::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region CustomMethods

void _MIDITrack::changeRepeatingMode(bool newRepeatState) {
    isRepeating = newRepeatState;
}

void _MIDITrack::changeFileTempo(float newTempo) {
    setTempoRatio = newTempo;
    rbbStretcher->setTimeRatio(1 / setTempoRatio);
}

void _MIDITrack::SetMIDITemplateSamplingProvider(_IEngine* audioProvider) {
    templateSamplingAudioProvider = audioProvider;
}

void _MIDITrack::RenderMIDIWaveform(float* notesPitchRatioArr, int count) {
    //MIDITrackBuffer.clear();

    int samplesPerUnit = (100 * spec.sampleRate) / 1000;
    int numSampleProcessBlocks = ceil((double)samplesPerUnit / 512);

    templateSamplingAudioProvider->setBlockSize(samplesPerUnit);

    MIDITrackBuffer.setSize(1, samplesPerUnit * count);

    int xcounter = 0;
    for (int i = 0; i < count; i++) {
        juce::AudioBuffer<float> planarBuffer;
        planarBuffer.setSize(1, samplesPerUnit);
        juce::AudioSourceChannelInfo bufferToFill(&planarBuffer, 0, samplesPerUnit);

        templateSamplingAudioProvider->getNextAudioBlock(bufferToFill);
        MIDITrackBuffer.addFrom(0, samplesPerUnit * i, bufferToFill.buffer->getReadPointer(0, 0), samplesPerUnit);
    }
    printf("dfkgsdkfas");
    //UNFINISHED
}

/*float _MIDITrack::resampleSample(int channelIndex, float sampleIndex, float _pitchRatio)
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
}*/

#pragma endregion