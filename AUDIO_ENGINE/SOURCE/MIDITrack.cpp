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

void _MIDITrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) {}

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

float _MIDITrack::resampleSample(int channelIndex, float sampleIndex, float _pitchRatio)
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