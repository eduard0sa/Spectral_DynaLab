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
    outputGain->setGainLinear(gain);
}

DSPEffect* _Oscillator::addDistortionDSPEffect() {
    DSPDistortionEffect* distortionEffectHEAP = (class DSPDistortionEffect*)malloc(sizeof(class DSPDistortionEffect));
    DSPDistortionEffect* distortionEffectSTACK = new (distortionEffectHEAP) DSPDistortionEffect(); //Stack reference of the memory allocated at the last row, used to call the DSPDistortionEffect's class constructor.
    
    Random randomizer = Random();
    int distortionEffectID;

    do {
        distortionEffectID = randomizer.nextInt(200);
    } while (checkExistantEffectID(distortionEffectID));

    distortionEffectHEAP->prepare(spec);
    distortionEffectHEAP->id = distortionEffectID;

    DSPEffectChain[DSPEffectChainLength] = distortionEffectHEAP;
    DSPEffectChainLength++;

    return distortionEffectHEAP;
}

void _Oscillator::removeDSPEffect(void* effect) {
    for (int i = 0; i < DSPEffectChainLength; i++) {
        if (DSPEffectChain[i]->getEffectID() == ((DSPEffect*)effect)->getEffectID()) {
            free(DSPEffectChain[i]);
            DSPEffectChain[i] = NULL;

            for (int j = i; j < DSPEffectChainLength; j++) {
                DSPEffectChain[j] = DSPEffectChain[j + 1];
            }

            DSPEffectChainLength--;
            break;
        }
    }
}

bool _Oscillator::checkExistantEffectID(int id) {
    for (int i = 0; i < DSPEffectChainLength; i++) {
        if (DSPEffectChain[i]->getEffectID() == id) {
            return true;
        }
    }
    return false;
}

#pragma endregion