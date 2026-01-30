#include "Oscillator.h"

_Oscillator::_Oscillator() {}

_Oscillator::~_Oscillator() {}

#pragma region Juce_Overriden_methods

float test(float sample) {
    return tanh(sample);
}

void _Oscillator::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    juce::dsp::ProcessSpec spec;
    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    DSP_EFFECTS->inputGain->prepare(spec);
    DSP_EFFECTS->outputGain->prepare(spec);
    DSP_EFFECTS->inputGain->setGainLinear(gain + distortionDrive);
    DSP_EFFECTS->outputGain->setGainLinear(gain);
    DSP_EFFECTS->distortion->prepare(spec);
    DSP_EFFECTS->outputGain->setRampDurationSeconds(0.02); // super important

    DSP_EFFECTS->distortion->functionToUse = [](float sample)
    {
        return test(sample);
    };
}

void _Oscillator::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill)
{
    bufferToFill.clearActiveBufferRegion();
    float originalPhase = phase;
    for (int channel = 0; channel < bufferToFill.buffer->getNumChannels(); ++channel)
    {
        float* buffer = bufferToFill.buffer->getWritePointer(channel, bufferToFill.startSample);
        phase = originalPhase;

        for (int sample = 0; sample < bufferToFill.numSamples; ++sample)
        {
            buffer[sample] = std::sin(phase);
            phase = fmod(phase + phaseIncrement, MathConstants<float>::twoPi);
        }
    }

    juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
    juce::dsp::ProcessContextReplacing<float> context(audioBlock);

    DSP_EFFECTS->inputGain->process(context);
    DSP_EFFECTS->distortion->process(context);
    DSP_EFFECTS->outputGain->process(context);
}

void _Oscillator::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region Custom_methods

void _Oscillator::changeFrequency(float newFrequency)
{
    frequency = newFrequency;
    phaseIncrement = (2.0 * juce::MathConstants<double>::pi * frequency) / _sampleRate;
}

void _Oscillator::changeGain(float newGain) {
    gain = newGain;
    DSP_EFFECTS->inputGain->setGainLinear(1 + distortionDrive);
    DSP_EFFECTS->outputGain->setGainLinear(gain);
}

void _Oscillator::changeDistortionDrive(float newDrive) {
    distortionDrive = newDrive;
    DSP_EFFECTS->inputGain->setGainLinear(gain + distortionDrive);
    DSP_EFFECTS->outputGain->setGainLinear(gain);
}

#pragma endregion