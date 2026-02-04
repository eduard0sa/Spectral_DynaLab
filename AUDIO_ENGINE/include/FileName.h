#pragma once

#ifdef AUDIO_ENGINE_EXPORTS
#define AUDIO_ENGINE_API __declspec(dllexport)
#else
#define AUDIO_ENGINE_API __declspec(dllimport)
#endif

#include <stdio.h>

using namespace std;

#pragma region EngineLogic

extern "C" AUDIO_ENGINE_API int _main_();
extern "C" AUDIO_ENGINE_API void* createEngine();
extern "C" AUDIO_ENGINE_API void enginePrepareToPlay(void* engine, double sampleRate, int samplesPerBlockExpected, float startFrequency, float startGain);
extern "C" AUDIO_ENGINE_API void engineProcessWave(void* engine, float* buffer, int numSamples);
extern "C" AUDIO_ENGINE_API void destroyEngine(void* engine);

extern "C" AUDIO_ENGINE_API void changeFrequency(void* engine, float newFrequency);
extern "C" AUDIO_ENGINE_API void changeGain(void* engine, float newGain);

#pragma endregion EngineLogic
#pragma region DSPLogic

extern "C" AUDIO_ENGINE_API void* addDSPEffect(void* engine, int dspType);
extern "C" AUDIO_ENGINE_API void removeDSPEffect(void* engine, void* effectDSPProcessor);

#pragma region DistortionDSP

extern "C" AUDIO_ENGINE_API void changeDistortionDrive(void* distortionDSPProcessor, float newDrive);
extern "C" AUDIO_ENGINE_API void changeDistortionFunctionToUse(void* distortionDSPProcessor, int newFunctionIndex);

#pragma endregion DistortionDSP
#pragma region CompressorDSP

extern "C" AUDIO_ENGINE_API void changeCompressorThreshold(void* compressorDSPProcessor, float newThreshold);
extern "C" AUDIO_ENGINE_API void changeCompressorRatio(void* compressorDSPProcessor, float newRatio);
extern "C" AUDIO_ENGINE_API void changeCompressorAttack(void* compressorDSPProcessor, float newAttack);
extern "C" AUDIO_ENGINE_API void changeCompressorRelease(void* compressorDSPProcessor, float newRelease);

#pragma endregion CompressorDSP
#pragma region ReverbDSP

extern "C" AUDIO_ENGINE_API void changeReverbRoomSize(void* reverbDSPProcessor, float newRoomSize);
extern "C" AUDIO_ENGINE_API void changeReverbDamping(void* reverbDSPProcessor, float newDamping);
extern "C" AUDIO_ENGINE_API void changeReverbWetLevel(void* reverbDSPProcessor, float newWetLevel);
extern "C" AUDIO_ENGINE_API void changeReverbDryLevel(void* reverbDSPProcessor, float newDryLevel);
extern "C" AUDIO_ENGINE_API void changeReverbWidth(void* reverbDSPProcessor, float newWidth);
extern "C" AUDIO_ENGINE_API void changeReverbFreezeMode(void* reverbDSPProcessor, bool newFreezeMode);

#pragma endregion ReverbDSP
#pragma endregion DSPLogic