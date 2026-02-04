#include "pch.h"
#include "AUDIO_PROC_BRIDGE.h"
#include "FileName.h"

#pragma region EngineMgmtLogic

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

#pragma endregion EngineMgmtLogic

#pragma region DSPs

IntPtr AUDIOPROCBRIDGE::AudioEngineRef::_addDSPEffect(IntPtr engine, int effectTypeID) {
	return IntPtr(addDSPEffect((void*)engine, effectTypeID));
}

void AUDIOPROCBRIDGE::AudioEngineRef::_removeDSPEffect(IntPtr engine, IntPtr effectDSPProcessor) {
	removeDSPEffect((void*)engine, (void*)effectDSPProcessor);
}

#pragma region DistortionDSP

void AUDIOPROCBRIDGE::AudioEngineRef::_changeDistortionDrive(IntPtr distortionDSPProcessor, float newDrive) {
	changeDistortionDrive((void*)distortionDSPProcessor, newDrive);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeDistortionFunctionToUse(IntPtr distortionDSPProcessor, float newFunctionIndex) {
	changeDistortionFunctionToUse((void*)distortionDSPProcessor, newFunctionIndex);
}

#pragma endregion DistortionDSP

#pragma region CompressorDSP

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorThreshold(IntPtr compressorDSPProcessor, float newThreshold) {
	changeCompressorThreshold((void*)compressorDSPProcessor, newThreshold);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorRatio(IntPtr compressorDSPProcessor, float newRatio) {
	changeCompressorThreshold((void*)compressorDSPProcessor, newRatio);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorAttack(IntPtr compressorDSPProcessor, float newAttack) {
	changeCompressorThreshold((void*)compressorDSPProcessor, newAttack);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorRelease(IntPtr compressorDSPProcessor, float newRelease) {
	changeCompressorThreshold((void*)compressorDSPProcessor, newRelease);
}

#pragma endregion CompressorDSP
#pragma endregion DSPs