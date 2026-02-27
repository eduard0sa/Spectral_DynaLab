#include "pch.h"
#include "AUDIO_PROC_BRIDGE.h"
#include "FileName.h"

#pragma region EngineMgmtLogic

int AUDIOPROCBRIDGE::AudioEngineRef::_root_() {
	return _main_();
}

IntPtr AUDIOPROCBRIDGE::AudioEngineRef::_createEngine(bool isMidi) {
	void* native = ::createEngine(isMidi);
    return IntPtr(native);
}

IntPtr AUDIOPROCBRIDGE::AudioEngineRef::_createAudioFileEngine(System::String^ path) {
	std:string stdStringTypePath = u8"";
	for (int i = 0; i < path->Length; i++) {
		stdStringTypePath += path[i];
	}

	void* native = ::createAudioFileEngine(stdStringTypePath);
	return IntPtr(native);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_enginePrepareToPlay(IntPtr engine, double sampleRate, int samplesPerBlockExpected) {
	enginePrepareToPlay((void*)engine, sampleRate, samplesPerBlockExpected, 50, 10);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_engineProcessWave(IntPtr engine, cli::array<float>^ buffer, int numSamples, int offset) {
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

void AUDIOPROCBRIDGE::AudioEngineRef::_changeWaveShapeFunction(IntPtr engine, int functionIndex) {
	changeWaveShapeFunction((void*)engine, functionIndex);
}

cli::array<float>^ AUDIOPROCBRIDGE::AudioEngineRef::_pushOscVisSamples(IntPtr engine) {
	cli::array<float>^ managedArray = gcnew cli::array<float>(512);
	float* visSamplesArrPTR = pushOscVisSamples((void*)engine);

	for (int i = 0; i < 512; i++) {
		managedArray[i] = visSamplesArrPTR[i];
	}

	return managedArray;
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeAudioFileRepeatingMode(IntPtr engine, bool newRepeatState) {
	changeAudioFileRepeatingMode((void*)engine, newRepeatState);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeAudioFileTimePitchCouplingMode(IntPtr engine, bool newTimePitchCouplingMode) {
	changeAudioFileTimePitchCouplingMode((void*)engine, newTimePitchCouplingMode);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeAudioFileTempo(IntPtr engine, float newTempo) {
	changeAudioFileTempo((void*)engine, newTempo);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeAudioFilePitch(IntPtr engine, float newPitch) {
	changeAudioFilePitch((void*)engine, newPitch);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeMIDITrackRepeatingMode(IntPtr engine, bool newRepeatState) {
	changeMIDITrackRepeatingMode((void*)engine, newRepeatState);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeMIDITrackTempo(IntPtr engine, float newTempo) {
	changeMIDITrackTempo((void*)engine, newTempo);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_setMIDITemplateSamplingProvider(IntPtr engine, IntPtr audioProvider) {
	setMIDITemplateSamplingProvider((void*)engine, (void*)audioProvider);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_renderMIDIWaveform(IntPtr engine, cli::array<float>^ notesPitchRatioArr, int count) {
	pin_ptr<float> pinnedPtr = &notesPitchRatioArr[0];
	// Now you can safely get a float* to native code
	float* nativePtr = pinnedPtr;
	renderMIDIWaveform((void*)engine, nativePtr, count);
}

#pragma endregion EngineMgmtLogic

#pragma region DSPs

cli::array<float>^ AUDIOPROCBRIDGE::AudioEngineRef::_pushVisSamples(IntPtr SFXDSPProcessor, int effectTypeID) {
	cli::array<float>^ managedArray = gcnew cli::array<float>(512);
	float* visSamplesArrPTR = pushVisSamples((void*)SFXDSPProcessor, effectTypeID);

	for (int i = 0; i < 512; i++) {
		managedArray[i] = visSamplesArrPTR[i];
	}

	return managedArray;
}

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
	changeCompressorRatio((void*)compressorDSPProcessor, newRatio);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorAttack(IntPtr compressorDSPProcessor, float newAttack) {
	changeCompressorAttack((void*)compressorDSPProcessor, newAttack);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeCompressorRelease(IntPtr compressorDSPProcessor, float newRelease) {
	changeCompressorRelease((void*)compressorDSPProcessor, newRelease);
}

#pragma endregion CompressorDSP

#pragma region ReverbDSP

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbRoomSize(IntPtr reverbDSPProcessor, float newRoomSize) {
	changeReverbRoomSize((void*)reverbDSPProcessor, newRoomSize);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbDamping(IntPtr reverbDSPProcessor, float newDamping) {
	changeReverbDamping((void*)reverbDSPProcessor, newDamping);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbWetLevel(IntPtr reverbDSPProcessor, float newWetLevel) {
	changeReverbWetLevel((void*)reverbDSPProcessor, newWetLevel);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbDryLevel(IntPtr reverbDSPProcessor, float newDryLevel) {
	changeReverbDryLevel((void*)reverbDSPProcessor, newDryLevel);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbWidth(IntPtr reverbDSPProcessor, float newWidth) {
	changeReverbWidth((void*)reverbDSPProcessor, newWidth);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeReverbFreezeMode(IntPtr reverbDSPProcessor, bool newFreezeMode) {
	changeReverbFreezeMode((void*)reverbDSPProcessor, newFreezeMode);
}

#pragma endregion ReverbDSP

#pragma region ChorusDSP

void AUDIOPROCBRIDGE::AudioEngineRef::_changeChorusRate(IntPtr chorusDSPProcessor, float newRate) {
	changeChorusRate((void*)chorusDSPProcessor, newRate);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeChorusDepth(IntPtr chorusDSPProcessor, float newDepth) {
	changeChorusDepth((void*)chorusDSPProcessor, newDepth);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeChorusCenterDelay(IntPtr chorusDSPProcessor, float newCenterDelay) {
	changeChorusCenterDelay((void*)chorusDSPProcessor, newCenterDelay);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeChorusFeedback(IntPtr chorusDSPProcessor, float newFeedback) {
	changeChorusFeedback((void*)chorusDSPProcessor, newFeedback);
}

void AUDIOPROCBRIDGE::AudioEngineRef::_changeChorusMix(IntPtr chorusDSPProcessor, float newMix) {
	changeChorusMix((void*)chorusDSPProcessor, newMix);
}

#pragma endregion ChorusDSP

#pragma endregion DSPs