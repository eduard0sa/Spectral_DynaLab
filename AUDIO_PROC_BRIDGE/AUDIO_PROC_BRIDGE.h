#pragma once

using namespace System;

namespace AUDIOPROCBRIDGE {
	public ref class AudioEngineRef
	{
	public:
		int _root_();
		IntPtr _createEngine();
		void _enginePrepareToPlay(IntPtr engine, double sampleRate, int samplesPerBlockExpected);
		void _engineProcessWave(IntPtr engine, array<float>^ buffer, int numSamples, int offset);
		void _destroyEngine(IntPtr engine);

		void _changeFrequency(IntPtr engine, float newFrequency);
		void _changeGain(IntPtr engine, float newGain);

		IntPtr _addDSPEffect(IntPtr engine, int effectTypeID);
		void _removeDSPEffect(IntPtr engine, IntPtr effectDSPProcessor);

		array<float>^ _pushVisSamples(IntPtr distortionDSPProcessor);
		void _changeDistortionDrive(IntPtr distortionDSPProcessor, float newDrive);
		void _changeDistortionFunctionToUse(IntPtr distortionDSPProcessor, float newFunctionIndex);

		void _changeCompressorThreshold(IntPtr compressorDSPProcessor, float newThreshold);
		void _changeCompressorRatio(IntPtr compressorDSPProcessor, float newRatio);
		void _changeCompressorAttack(IntPtr compressorDSPProcessor, float newAttack);
		void _changeCompressorRelease(IntPtr compressorDSPProcessor, float newRelease);

		void _changeReverbRoomSize(IntPtr reverbDSPProcessor, float newRoomSize);
		void _changeReverbDamping(IntPtr reverbDSPProcessor, float newDamping);
		void _changeReverbWetLevel(IntPtr reverbDSPProcessor, float newWetLevel);
		void _changeReverbDryLevel(IntPtr reverbDSPProcessor, float newDryLevel);
		void _changeReverbWidth(IntPtr reverbDSPProcessor, float newWidth);
		void _changeReverbFreezeMode(IntPtr reverbDSPProcessor, bool newFreezeMode);

		void _changeChorusRate(IntPtr chorusDSPProcessor, float newRate);
		void _changeChorusDepth(IntPtr chorusDSPProcessor, float newDepth);
		void _changeChorusCenterDelay(IntPtr chorusDSPProcessor, float newCenterDelay);
		void _changeChorusFeedback(IntPtr chorusDSPProcessor, float newFeedback);
		void _changeChorusMix(IntPtr chorusDSPProcessor, float newMix);
	};
}