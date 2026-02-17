#include "FileTrack.h"

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
}

void _FileTrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill)
{
    bufferToFill.clearActiveBufferRegion();

    if (currentSampleIndex < tempBuffer.getNumSamples()) {
        for (int channel = 0; channel < bufferToFill.buffer->getNumChannels(); ++channel)
        {
            float* buffer = bufferToFill.buffer->getWritePointer(channel, bufferToFill.startSample);

            for (int sample = 0; sample < min(bufferToFill.numSamples, tempBuffer.getNumSamples() - currentSampleIndex); ++sample)
            {
                buffer[sample] = tempBuffer.getSample(channel, currentSampleIndex + sample);
            }
        }

        currentSampleIndex += bufferToFill.numSamples;

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
    else if (isRepeating == true) {
        currentSampleIndex = 0;
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

#pragma endregion