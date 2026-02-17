#include "Oscillator.h"

_Oscillator::_Oscillator() {
    waveShape = enum_OscillatorWaveShapeType::sine;
    visSampleArrayHEAP = (float*)malloc(sizeof(float[512]));
    visSampleArraySTACK = new (visSampleArrayHEAP) float[512]();
}

_Oscillator::~_Oscillator() {
    int count = 0;

    while (count < DSPEffectChainLength) {
        removeDSPEffect(DSPEffectChain[count]);
        count++;
    }
}

#pragma region Juce_Overriden_methods

void _Oscillator::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    outputGain->prepare(spec);
    outputGain->setGainLinear(gain);
    outputGain->setRampDurationSeconds(0.02); // super important
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
            buffer[sample] = processWaveShapeFunction(phase);
            phase = fmod(phase + phaseIncrement, MathConstants<float>::twoPi);
        }
    }

    juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
    juce::dsp::ProcessContextReplacing<float> context(audioBlock);

    for (int i = 0; i < DSPEffectChainLength; i++) {
        DSPEffectChain[i]->process(context);
    }

    outputGain->process(context);

    for (int i = 0; i < 512; i++) {
        visSampleArraySTACK[i] = context.getOutputBlock().getChannelPointer(0)[i];
    }
}

void _Oscillator::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region Custom_methods

float _Oscillator::processWaveShapeFunction(float phase) {
    switch (waveShape) {
        case enum_OscillatorWaveShapeType::sine:
            return std::sin(phase);
        case enum_OscillatorWaveShapeType::square:
            return (2 * ceil(sin(phase))) - 1; //abs(sin(x))/sin(x) is similar, but can't divide by zero.
        case enum_OscillatorWaveShapeType::triangle:
            return 2 * abs(2 * ((1/MathConstants<float>::pi) * phase - floor(1/2 + (1/MathConstants<float>::pi) * phase))) - 1;
    }
}

void _Oscillator::changeFrequency(float newFrequency)
{
    frequency = newFrequency;
    phaseIncrement = (2.0 * juce::MathConstants<double>::pi * frequency) / _sampleRate;
}

void _Oscillator::changeWaveShapeFunction(enum_OscillatorWaveShapeType functionType) {
    waveShape = functionType;
}

#pragma endregion