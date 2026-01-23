#include "pch.h"
#include "AUDIO_PROC_BRIDGE.h"
#include "FileName.h"

int AUDIOPROCBRIDGE::AudioEngineRef::_root_() {
	return _main_();
}

IntPtr AUDIOPROCBRIDGE::AudioEngineRef::_createEngine() {
	void* native = ::createEngine();
    return IntPtr(native);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_enginePrepareToPlay(IntPtr engine, double sampleRate, int samplesPerBlockExpected) {
	enginePrepareToPlay((void*)engine, sampleRate, samplesPerBlockExpected, 50, 10);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_engineProcessWave(IntPtr engine, array<float>^ buffer, int numSamples, int offset) {
	pin_ptr<float> pinnedPtr = &buffer[offset];
	// Now you can safely get a float* to native code
	float* nativePtr = pinnedPtr;
	engineProcessWave((void*)engine, nativePtr, numSamples);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_destroyEngine(IntPtr engine) {
	destroyEngine((void*)engine);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeFrequency(IntPtr engine, float newFrequency) {
	changeFrequency((void*)engine, newFrequency);
}
void AUDIOPROCBRIDGE::AudioEngineRef::_changeGain(IntPtr engine, float newGain) {
	changeGain((void*)engine, newGain);
}