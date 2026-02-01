#include "FileName.h"
#include <../SOURCE/Oscillator.h>

extern "C" {
    int _main_() {
        return 3301;
    }

    void* createEngine() {
        return new _Oscillator();
	}

    void enginePrepareToPlay(void* engine, double sampleRate, int samplesPerBlockExpected, float startFrequency, float startGain) {
        ((_Oscillator*)engine)->prepareToPlay(samplesPerBlockExpected, sampleRate, startFrequency, startGain);
	}

    void engineProcessWave(void* engine, float* buffer, int numSamples) {
        juce::AudioBuffer<float> planarBuffer(&buffer, 1, numSamples);
        juce::AudioSourceChannelInfo bufferToFill(&planarBuffer, 0, numSamples);
        ((_Oscillator*)engine)->getNextAudioBlock(bufferToFill);
	}

    void destroyEngine(void* engine) {
        delete (_Oscillator*)engine;
    }

    void changeFrequency(void* engine, float newFrequency) {
        ((_Oscillator*)engine)->changeFrequency(newFrequency);
	}

    void changeGain(void* engine, float newGain) {
        ((_Oscillator*)engine)->changeGain(newGain);
	}

    void* addDistortionDSPEffect(void* engine) {
        return ((_Oscillator*)engine)->addDistortionDSPEffect();
    }

    void removeDSPEffect(void* engine, void* effectDSPProcessor) {
        ((_Oscillator*)engine)->removeDSPEffect(effectDSPProcessor);
    }

    void changeDistortionDrive(void* distortionDSPProcessor, float newDrive) {
        ((DSPDistortionEffect*)distortionDSPProcessor)->changeDistortionDrive(newDrive);
    }
}