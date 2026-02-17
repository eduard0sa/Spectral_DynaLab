#include "FileName.h"
#include <../SOURCE/Oscillator.h>
#include <../SOURCE/FileTrack.h>
#include <..\SOURCE\WaveEngineTemplate.h>

extern "C" {
    int _main_() {
        return 3301;
    }

    #pragma region EngineMgmtLogic

    void* createEngine() {
        return new _Oscillator();
	}

    void* createAudioFileEngine(std::string filePath) {
        return new _FileTrack(filePath);
    }

    void enginePrepareToPlay(void* engine, double sampleRate, int samplesPerBlockExpected, float startFrequency, float startGain) {
        ((_IEngine*)engine)->prepareToPlay(samplesPerBlockExpected, sampleRate, startFrequency, startGain);
	}

    void engineProcessWave(void* engine, float* buffer, int numSamples) {
        juce::AudioBuffer<float> planarBuffer(&buffer, 1, numSamples);
        juce::AudioSourceChannelInfo bufferToFill(&planarBuffer, 0, numSamples);
        ((_IEngine*)engine)->getNextAudioBlock(bufferToFill);
	}

    void destroyEngine(void* engine) {
        delete (_IEngine*)engine;
    }

    float* pushOscVisSamples(void* engine) {
        return ((_IEngine*)engine)->pushOscVisSamples();
    }

    void changeFrequency(void* engine, float newFrequency) {
        ((_Oscillator*)engine)->changeFrequency(newFrequency);
	}

    void changeGain(void* engine, float newGain) {
        ((_IEngine*)engine)->changeGain(newGain);
	}

    void changeWaveShapeFunction(void* engine, int functionIndex) {
        switch (functionIndex) {
            case 0:
                ((_Oscillator*)engine)->changeWaveShapeFunction(enum_OscillatorWaveShapeType::sine);
                break;
            case 1:
                ((_Oscillator*)engine)->changeWaveShapeFunction(enum_OscillatorWaveShapeType::square);
                break;
            case 2:
                ((_Oscillator*)engine)->changeWaveShapeFunction(enum_OscillatorWaveShapeType::triangle);
                break;
            }
    }

    void changeAudioFileRepeatingMode(void* engine, bool newRepeatState) {
        ((_FileTrack*)engine)->changeRepeatingMode(newRepeatState);
    }

    #pragma endregion EngineMgmtLogic

    #pragma region DSPs

    void* addDSPEffect(void* engine, int dspType) {
        switch (dspType) {
            case enum_EffectType::Distortion:
                return ((_IEngine*)engine)->addDSPEffect<DSPDistortionEffect>();
            case enum_EffectType::Compressor:
                return ((_IEngine*)engine)->addDSPEffect<DSPCompressorEffect>();
            case enum_EffectType::Reverb:
                return ((_IEngine*)engine)->addDSPEffect<DSPReverbEffect>();
            case enum_EffectType::Chorus:
                return ((_IEngine*)engine)->addDSPEffect<DSPChorusEffect>();
            default:
                return NULL;
        }
    }

    void removeDSPEffect(void* engine, void* effectDSPProcessor) {
        ((_IEngine*)engine)->removeDSPEffect(effectDSPProcessor);
    }

    #pragma region DistortionDSP

    float* pushVisSamples(void* SFXDSPProcessor, int dspType) {
        float* a = NULL;

        switch (dspType) {
        case enum_EffectType::Distortion:
            a = ((DSPDistortionEffect*)SFXDSPProcessor)->pushVisSamples();
            break;
        case enum_EffectType::Compressor:
            a = ((DSPCompressorEffect*)SFXDSPProcessor)->pushVisSamples();
            break;
        case enum_EffectType::Reverb:
            a = ((DSPReverbEffect*)SFXDSPProcessor)->pushVisSamples();
            break;
        case enum_EffectType::Chorus:
            a = ((DSPChorusEffect*)SFXDSPProcessor)->pushVisSamples();
            break;
        }

        return a;
    }

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

    #pragma region ReverbDSP

    void changeReverbRoomSize(void* reverbDSPProcessor, float newRoomSize) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbRoomSize(newRoomSize);
    }

    void changeReverbDamping(void* reverbDSPProcessor, float newDamping) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbDamping(newDamping);
    }

    void changeReverbWetLevel(void* reverbDSPProcessor, float newWetLevel) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbWetLevel(newWetLevel);
    }

    void changeReverbDryLevel(void* reverbDSPProcessor, float newDryLevel) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbDryLevel(newDryLevel);
    }

    void changeReverbWidth(void* reverbDSPProcessor, float newWidth) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbWidth(newWidth);
    }

    void changeReverbFreezeMode(void* reverbDSPProcessor, bool newFreezeMode) {
        ((DSPReverbEffect*)reverbDSPProcessor)->changeReverbFreezeMode(newFreezeMode);
    }

    #pragma endregion ReverbDSP

    #pragma region ChorusDSP

    void changeChorusRate(void* chorusDSPProcessor, float newRate) {
        ((DSPChorusEffect*)chorusDSPProcessor)->changeChorusRate(newRate);
    }
    void changeChorusDepth(void* chorusDSPProcessor, float newDepth) {
        ((DSPChorusEffect*)chorusDSPProcessor)->changeChorusDepth(newDepth);
    }
    void changeChorusCenterDelay(void* chorusDSPProcessor, float newCenterDelay) {
        ((DSPChorusEffect*)chorusDSPProcessor)->changeChorusCenterDelay(newCenterDelay);
    }
    void changeChorusFeedback(void* chorusDSPProcessor, float newFeedback) {
        ((DSPChorusEffect*)chorusDSPProcessor)->changeChorusFeedback(newFeedback);
    }
    void changeChorusMix(void* chorusDSPProcessor, float newMix) {
        ((DSPChorusEffect*)chorusDSPProcessor)->changeChorusMix(newMix);
    }

    #pragma endregion ChorusDSP

    #pragma endregion DSPs
}