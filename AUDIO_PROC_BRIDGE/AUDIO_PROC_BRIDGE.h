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
		IntPtr _addDistortionDSPEffect(IntPtr engine);
		void _removeDSPEffect(IntPtr engine, IntPtr effectDSPProcessor);
		void _changeDistortionDrive(IntPtr distortionDSPProcessor, float newDrive);
		void _changeDistortionFunctionToUse(IntPtr distortionDSPProcessor, float newFunctionIndex);
	};
}