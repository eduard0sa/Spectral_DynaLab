#include "FileTrack.h"

_FileTrack::_FileTrack() {
    File audioFile;
    audioFile = File::getSpecialLocation(File::SpecialLocationType::userMusicDirectory).getChildFile("borne - Take Away [NCS Release].mp3");
    if (audioFile.existsAsFile()) {
        auto* reader = formatManager->createReaderFor(audioFile);

        if (reader != nullptr) {
            readerSource.reset(new juce::AudioFormatReaderSource(reader, true));
            transportSource->setSource(readerSource.get(),
                0, // no buffer size restriction
                nullptr, // use default reader
                reader->sampleRate);

            changeGain(0.5f);
            transportSource->start();
        }
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
}

void _FileTrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill)
{
    bufferToFill.clearActiveBufferRegion();
    float originalPhase = phase;
    for (int channel = 0; channel < bufferToFill.buffer->getNumChannels(); ++channel)
    {
        float* buffer = bufferToFill.buffer->getWritePointer(channel, bufferToFill.startSample);
        phase = originalPhase;

        for (int sample = 0; sample < bufferToFill.numSamples; ++sample)
        {
            transportSource->getNextAudioBlock(bufferToFill);
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

float* _FileTrack::pushOscVisSamples() {
    return visSampleArrayHEAP;
}

void _FileTrack::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region Custom_methods

void _FileTrack::changeGain(float newGain) {
    gain = newGain;
    outputGain->setGainLinear(gain);
}

void _FileTrack::removeDSPEffect(void* effect) {
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

bool _FileTrack::checkExistantEffectID(int id) {
    for (int i = 0; i < DSPEffectChainLength; i++) {
        if (DSPEffectChain[i]->getEffectID() == id) {
            return true;
        }
    }
    return false;
}

#pragma endregion