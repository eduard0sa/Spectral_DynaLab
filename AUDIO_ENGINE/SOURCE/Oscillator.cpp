#include "Oscillator.h"

_Oscillator::_Oscillator() {}

_Oscillator::~_Oscillator() {}

#pragma region Juce_Overriden_methods

void _Oscillator::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    addDistortionDSPEffect();
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
            buffer[sample] = std::sin(phase);
            phase = fmod(phase + phaseIncrement, MathConstants<float>::twoPi);
        }
    }

    juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
    juce::dsp::ProcessContextReplacing<float> context(audioBlock);

    for (int i = 0; i < DSPEffectChainLength; i++) {
        DSPEffectChain[i]->process(context);
    }

    /*DSP_EFFECTS->inputGain->process(context);
    DSP_EFFECTS->distortion->process(context);*/
    outputGain->process(context);
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
    outputGain->setGainLinear(gain);
}

void _Oscillator::changeDistortionDrive(float newDrive) {
    distortionDrive = newDrive;
    DSP_EFFECTS->inputGain->setGainLinear(gain + distortionDrive);
    outputGain->setGainLinear(gain);
}

void _Oscillator::addDistortionDSPEffect() {
    unique_ptr<dsp::Gain<float>> inputGain = make_unique<dsp::Gain<float>>();
	unique_ptr<dsp::WaveShaper<float>> distortionEffect = make_unique<dsp::WaveShaper<float>>();
    
    inputGain->prepare(spec);
    inputGain->setGainLinear(1 + distortionDrive);
    distortionEffect->prepare(spec);

    distortionEffect->functionToUse = [](float sample)
    {
        return tanh(sample);
    };

    unique_ptr<DSPGainEffect> inputGainEffect = make_unique<DSPGainEffect>();
    inputGainEffect->effectPtr = std::move(inputGain);
    
    unique_ptr<DSPDistortionEffect> distortionDSPEffect = make_unique<DSPDistortionEffect>();
	distortionDSPEffect->effectPtr = std::move(distortionEffect);

	DSPEffectChain[DSPEffectChainLength] = std::move(inputGainEffect);
    DSPEffectChainLength++;
    DSPEffectChain[DSPEffectChainLength] = std::move(distortionDSPEffect);
    DSPEffectChainLength++;
}

#pragma endregion