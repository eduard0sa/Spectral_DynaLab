#include "MIDITrack.h"

using namespace juce;
using namespace std;

_MIDITrack::_MIDITrack() {
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

    int options =
        RubberBand::RubberBandStretcher::OptionProcessRealTime |
        RubberBand::RubberBandStretcher::OptionThreadingNever |
        RubberBand::RubberBandStretcher::OptionPitchHighQuality;

    rbbStretcher = std::make_unique<RubberBand::RubberBandStretcher>(spec.sampleRate, numChannels, options);
    changeFileTempo(setTempoRatio);
}

void _MIDITrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray = true) {
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

        if (fillVisualizationArray) {
            for (int i = 0; i < spec.maximumBlockSize; i++) {
                visSampleArraySTACK[i] = context.getOutputBlock().getChannelPointer(0)[i];
            }
        }
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

void _MIDITrack::RenderMIDIWaveform(std::vector<std::vector<struct_noteInfo>> notesPitchRatioArr, int notesCount, int maxNotesPerColumn) {
    MIDITrackBuffer.clear();

    samplesPerNoteUnit = (100 * spec.sampleRate) / 1000;

    MIDITrackBuffer.setSize(1, samplesPerNoteUnit * notesCount);

    float* buffer = MIDITrackBuffer.getWritePointer(0, 0);

    for (int i = 0; i < notesCount; i++) {
        for (int j = 0; j < notesPitchRatioArr[i].size(); j++) {
            int bufferLength = samplesPerNoteUnit * notesPitchRatioArr[i][j].duration;

            juce::AudioBuffer<float> currentNotePlanarBuffer = juce::AudioBuffer<float>(1, bufferLength);
            juce::AudioSourceChannelInfo bufferToFill = juce::AudioSourceChannelInfo(&currentNotePlanarBuffer, 0, bufferLength);

            if(templateSamplingAudioProvider->getEngineType() == "FILETRACK") ((_FileTrack)templateSamplingAudioProvider)->resetTime();
            templateSamplingAudioProvider->setBlockSize(bufferLength);

            bufferToFill.buffer->clear();
            templateSamplingAudioProvider->getNextAudioBlock(bufferToFill, false);
            
			processFrequencyChange(bufferToFill, notesPitchRatioArr[i][j].pitchRatio);
            
            for (int k = 0; k < bufferLength; k++) {
				buffer[i * samplesPerNoteUnit + k] += (float)bufferToFill.buffer->getSample(0, k);
            }
        }
    }
}

void _MIDITrack::processFrequencyChange(const juce::AudioSourceChannelInfo& bufferToFill, float pitchRatio) {
	rbbStretcher->setPitchScale(pitchRatio);

    rbbStretcher->reset();

    const int numChannels = bufferToFill.buffer->getNumChannels();
    const int requestedSamples = bufferToFill.numSamples;

    int availableInput = requestedSamples;
    int readIndex = 0;

    std::vector<const float*> input(numChannels);

    int blockToFeed = bufferToFill.buffer->getNumSamples();

    for (int ch = 0; ch < numChannels; ++ch)
        input[ch] = bufferToFill.buffer->getReadPointer(ch, readIndex);

    rbbStretcher->process(input.data(), blockToFeed, false);

    readIndex += blockToFeed;

    int availableOutput = rbbStretcher->available();

    if (availableOutput > 0)
    {
        int samplesToRetrieve = min(requestedSamples, availableOutput);

        std::vector<float*> output(numChannels);

        for (int ch = 0; ch < numChannels; ++ch)
            //output[ch] = bufferToFill.buffer->getWritePointer(ch, bufferToFill.startSample);
            output[ch] = bufferToFill.buffer->getWritePointer(ch, 0);

        rbbStretcher->retrieve(output.data(), samplesToRetrieve);
    }
}

string _MIDITrack::getEngineType() {
    return "MIDITRACK";
}

#pragma endregion