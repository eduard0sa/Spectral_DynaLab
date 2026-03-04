#pragma once

#ifdef AUDIO_ENGINE_EXPORTS
#define AUDIO_ENGINE_API __declspec(dllexport)
#else
#define AUDIO_ENGINE_API __declspec(dllimport)
#endif

#include <stdio.h>
#include <string>

using namespace std;

#pragma region EngineLogic

extern "C" AUDIO_ENGINE_API int _main_();
extern "C" AUDIO_ENGINE_API void* createEngine(bool isMidi);
extern "C" AUDIO_ENGINE_API void* createAudioFileEngine(std::string filePath);
extern "C" AUDIO_ENGINE_API void enginePrepareToPlay(void* engine, double sampleRate, int samplesPerBlockExpected, float startFrequency, float startGain);
extern "C" AUDIO_ENGINE_API void engineProcessWave(void* engine, float* buffer, int numSamples);
extern "C" AUDIO_ENGINE_API void destroyEngine(void* engine);

extern "C" AUDIO_ENGINE_API float* pushOscVisSamples(void* engine);

extern "C" AUDIO_ENGINE_API void changeFrequency(void* engine, float newFrequency);
extern "C" AUDIO_ENGINE_API void changeGain(void* engine, float newGain);
extern "C" AUDIO_ENGINE_API void changeWaveShapeFunction(void* engine, int functionIndex);

extern "C" AUDIO_ENGINE_API void changeAudioFileRepeatingMode(void* engine, bool newRepeatState);
extern "C" AUDIO_ENGINE_API void changeAudioFileTimePitchCouplingMode(void* engine, bool newTimePitchCouplingMode);
extern "C" AUDIO_ENGINE_API void changeAudioFileTempo(void* engine, float newTempo);
extern "C" AUDIO_ENGINE_API void changeAudioFilePitch(void* engine, float newPitch);

extern "C" AUDIO_ENGINE_API void changeMIDITrackRepeatingMode(void* engine, bool newRepeatState);
extern "C" AUDIO_ENGINE_API void changeMIDITrackTempo(void* engine, float newTempo);
extern "C" AUDIO_ENGINE_API void setMIDITemplateSamplingProvider(void* engine, void* audioProvider);
extern "C" AUDIO_ENGINE_API void renderMIDIWaveform(void* engine, float** notesPitchRatioArr, int count);

#pragma endregion EngineLogic
#pragma region DSPLogic

extern "C" AUDIO_ENGINE_API void* addDSPEffect(void* engine, int dspType);
extern "C" AUDIO_ENGINE_API void removeDSPEffect(void* engine, void* effectDSPProcessor);
extern "C" AUDIO_ENGINE_API float* pushVisSamples(void* SFXDSPProcessor, int dspType);

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
#pragma region ChorusDSP

extern "C" AUDIO_ENGINE_API void changeChorusRate(void* chorusDSPProcessor, float newRate);
extern "C" AUDIO_ENGINE_API void changeChorusDepth(void* chorusDSPProcessor, float newDepth);
extern "C" AUDIO_ENGINE_API void changeChorusCenterDelay(void* chorusDSPProcessor, float newCenterDelay);
extern "C" AUDIO_ENGINE_API void changeChorusFeedback(void* chorusDSPProcessor, float newFeedback);
extern "C" AUDIO_ENGINE_API void changeChorusMix(void* chorusDSPProcessor, float newMix);

#pragma endregion ChorusDSP

#pragma endregion DSPLogic