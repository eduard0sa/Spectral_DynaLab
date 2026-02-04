#include "FileName.h"
#include <../SOURCE/Oscillator.h>

extern "C" {
    int _main_() {
        return 3301;
    }

    #pragma region EngineMgmtLogic

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

    #pragma endregion EngineMgmtLogic

    #pragma region DSPs

    void* addDSPEffect(void* engine, int dspType) {
        switch (dspType) {
            case enum_EffectType::Distortion:
                return ((_Oscillator*)engine)->addDSPEffect<DSPDistortionEffect>();
            case enum_EffectType::Compressor:
                return ((_Oscillator*)engine)->addDSPEffect<DSPCompressorEffect>();
            default:
                return NULL;
        }
    }

    void removeDSPEffect(void* engine, void* effectDSPProcessor) {
        ((_Oscillator*)engine)->removeDSPEffect(effectDSPProcessor);
    }

    #pragma region DistortionDSP

    void changeDistortionDrive(void* distortionDSPProcessor, float newDrive) {
        ((DSPDistortionEffect*)distortionDSPProcessor)->changeDistortionDrive(newDrive);
    }

    void changeDistortionFunctionToUse(void* distortionDSPProcessor, int newFunctionIndex) {
        switch (newFunctionIndex) {
            case 0:
                ((DSPDistortionEffect*)distortionDSPProcessor)->changeFunctionToUse([](float sample)
                    {
                        return tanh(sample);
                    });
                break;
            case 1:
                ((DSPDistortionEffect*)distortionDSPProcessor)->changeFunctionToUse([](float sample)
                    {
                        float limit = 1;

                        if (sample > limit) {
                            return limit;
                        }
                        else {
                            return sample;
                        }
                    });
                break;
            case 2:
                ((DSPDistortionEffect*)distortionDSPProcessor)->changeFunctionToUse([](float sample)
                    {
                        float limit = 1;

                        if (sample > limit) {
                            return sample - limit;
                        }
                        else {
                            return sample;
                        }
                    });
                break;
        }
    }

    #pragma endregion DistortionDSP

    #pragma region CompressorDSP

    void changeCompressorThreshold(void* compressorDSPProcessor, float newThreshold) {
        ((DSPCompressorEffect*)compressorDSPProcessor)->changeCompressorThreshold(newThreshold);
    }

    void changeCompressorRatio(void* compressorDSPProcessor, float newRatio) {
        ((DSPCompressorEffect*)compressorDSPProcessor)->changeCompressorRatio(newRatio);
    }

    void changeCompressorAttack(void* compressorDSPProcessor, float newAttack) {
        ((DSPCompressorEffect*)compressorDSPProcessor)->changeCompressorAttack(newAttack);
    }

    void changeCompressorRelease(void* compressorDSPProcessor, float newRelease) {
        ((DSPCompressorEffect*)compressorDSPProcessor)->changeCompressorRelease(newRelease);
    }

    #pragma endregion CompressorDSP

    #pragma endregion DSPs
}