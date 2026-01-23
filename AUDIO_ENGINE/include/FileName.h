#pragma once

#ifdef AUDIO_ENGINE_EXPORTS
#define AUDIO_ENGINE_API __declspec(dllexport)
#else
#define AUDIO_ENGINE_API __declspec(dllimport)
#endif

#include <stdio.h>
using namespace std;

extern "C" AUDIO_ENGINE_API int _main_();
extern "C" AUDIO_ENGINE_API void* createEngine();
extern "C" AUDIO_ENGINE_API void enginePrepareToPlay(void* engine, double sampleRate, int samplesPerBlockExpected, float startFrequency, float startGain);
extern "C" AUDIO_ENGINE_API void engineProcessWave(void* engine, float* buffer, int numSamples);
extern "C" AUDIO_ENGINE_API void destroyEngine(void* engine);

extern "C" AUDIO_ENGINE_API void changeFrequency(void* engine, float newFrequency);
extern "C" AUDIO_ENGINE_API void changeGain(void* engine, float newGain);