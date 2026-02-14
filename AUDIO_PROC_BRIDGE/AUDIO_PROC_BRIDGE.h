#pragma once

using namespace System;

namespace AUDIOPROCBRIDGE {
	/// <summary>
	/// JUCE AudioEngine Bridge Wrapper Class.
	/// </summary>
	public ref class AudioEngineRef
	{
	public:
		int _root_();

		#pragma region EngineLogic

		/// <summary>
		/// This method creates an oscillator engine in the core audio engine.
		/// </summary>
		/// <returns>The int pointer to the new oscillator engine.</returns>
		/// 
		IntPtr _createEngine();

		/// <summary>
		/// This method prepares oscillator's parameters for play.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="sampleRate">The rate at which juce will process wave samples to be used in the main app.</param>
		/// <param name="samplesPerBlockExpected">The number of samples per sample block.</param>
		void _enginePrepareToPlay(IntPtr engine, double sampleRate, int samplesPerBlockExpected);

		/// <summary>
		/// This method processes the sound wave to be read by the main app.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="buffer">The empty float array buffer to be filled by JUCE.</param>
		/// <param name="numSamples">Number of samples to process.</param>
		/// <param name="offset">Sample array index from where JUCE starts processing the wave data.</param>
		void _engineProcessWave(IntPtr engine, array<float>^ buffer, int numSamples, int offset);

		/// <summary>
		/// This method is executed when the Oscillator engine instance is removed, killing all dynamic components associated, avoiding memory leaks.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		void _destroyEngine(IntPtr engine);

		/// <summary>
		/// This method changes an oscillator engine's wave frequency.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="newFrequency">New frequency value</param>
		void _changeFrequency(IntPtr engine, float newFrequency);

		/// <summary>
		/// This method changes an oscillator engine's wave gain.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="newGain">New gain value.</param>
		void _changeGain(IntPtr engine, float newGain);

		/// <summary>
		/// This method change sthe wave shape type to be played by juce (sinewave, squarewave, trianglewave).
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="functionIndex">New wave function index (maps to an internal enum).</param>
		void _changeWaveShapeFunction(IntPtr engine, int functionIndex);


		/// <summary>
		/// This method gets the oscillator wave samples array, for graphic visualization.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <returns>A floating-point sample array buffer, containing all samples to be displayed in a graph.</returns>
		array<float>^ _pushOscVisSamples(IntPtr engine);

		#pragma endregion EngineLogic

		#pragma region DSPLogic

		/// <summary>
		/// This method instantiates and allocates a new DSP Effect object into an oscillator engine effects chain.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="effectTypeID">New DSP effect processor type index (maps to an internal enum).</param>
		/// <returns>The integer pointer to the newly created DSP effect.</returns>
		IntPtr _addDSPEffect(IntPtr engine, int effectTypeID);

		/// <summary>
		/// This method removes a DSP effect object from an oscillator, freeing the allocated memory associated with it.
		/// </summary>
		/// <param name="engine">Pointer reference for the oscillator engine object in memory.</param>
		/// <param name="effectDSPProcessor">Pointer reference for the DSP Effect object previously allocated in memory.</param>
		void _removeDSPEffect(IntPtr engine, IntPtr effectDSPProcessor);

		/// <summary>
		/// This method gets the wave samples processed by a given DSP Effect engine, for graphic visualization.
		/// </summary>
		/// <param name="SFXDSPProcessor">Pointer reference for the DSP Effect object previously allocated in memory.</param>
		/// <param name="effectTypeID">DSP effect processor type index (maps to an internal enum).</param>
		/// <returns>A floating-point DSP SFX sample array buffer, containing all samples to be displayed in a graph.</returns>
		array<float>^ _pushVisSamples(IntPtr SFXDSPProcessor, int effectTypeID);

		#pragma region DistortionDSP

		/// <summary>
		/// This method changes the distortion drive value associated with a given distortion DSP SFX.
		/// </summary>
		/// <param name="distortionDSPProcessor">Pointer reference for the distortion DSP Effect object previously allocated in memory.</param>
		/// <param name="newDrive">The new distortion drive value.</param>
		void _changeDistortionDrive(IntPtr distortionDSPProcessor, float newDrive);

		/// <summary>
		/// This method changes the distortion function associated with a given distortion DSP SFX object.
		/// </summary>
		/// <param name="distortionDSPProcessor">Pointer reference for the distortion DSP Effect object previously allocated in memory.</param>
		/// <param name="newFunctionIndex">The new distortion function to be used by JUCE.</param>
		void _changeDistortionFunctionToUse(IntPtr distortionDSPProcessor, float newFunctionIndex);

		#pragma endregion DistortionDSP

		#pragma region CompressorDSP

		/// <summary>
		/// This method changes the compressor threshold associated with a given compressor DSP SFX object.
		/// </summary>
		/// <param name="compressorDSPProcessor">Pointer reference for the compressor DSP Effect object previously allocated in memory.</param>
		/// <param name="newThreshold">The new threshold value.</param>
		void _changeCompressorThreshold(IntPtr compressorDSPProcessor, float newThreshold);

		/// <summary>
		/// This method changes the compressor ratio associated with a given compressor DSP SFX object.
		/// </summary>
		/// <param name="compressorDSPProcessor">Pointer reference for the compressor DSP Effect object previously allocated in memory.</param>
		/// <param name="newRatio">The new ratio value.</param>
		void _changeCompressorRatio(IntPtr compressorDSPProcessor, float newRatio);

		/// <summary>
		/// This method changes the compressor attack associated with a given compressor DSP SFX object.
		/// </summary>
		/// <param name="compressorDSPProcessor">Pointer reference for the compressor DSP Effect object previously allocated in memory.</param>
		/// <param name="newAttack">The new attack value.</param>
		void _changeCompressorAttack(IntPtr compressorDSPProcessor, float newAttack);

		/// <summary>
		/// This method changes the compressor release associated with a given compressor DSP SFX object.
		/// </summary>
		/// <param name="compressorDSPProcessor">Pointer reference for the compressor DSP Effect object previously allocated in memory.</param>
		/// <param name="newRelease">The new release value.</param>
		void _changeCompressorRelease(IntPtr compressorDSPProcessor, float newRelease);

		#pragma endregion CompressorDSP

		#pragma region ReverbDSP

		/// <summary>
		/// This method changes the reverb room size associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newRoomSize">The new room size value.</param>
		void _changeReverbRoomSize(IntPtr reverbDSPProcessor, float newRoomSize);

		/// <summary>
		/// This method changes the reverb damping associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newDamping">The new damping value.</param>
		void _changeReverbDamping(IntPtr reverbDSPProcessor, float newDamping);

		/// <summary>
		/// This method changes the reverb wet level associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newWetLevel">The new wet level value.</param>
		void _changeReverbWetLevel(IntPtr reverbDSPProcessor, float newWetLevel);

		/// <summary>
		/// This method changes the reverb dry level associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newDryLevel">The new dry level value.</param>
		void _changeReverbDryLevel(IntPtr reverbDSPProcessor, float newDryLevel);

		/// <summary>
		/// This method changes the reverb width associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newWidth">The new width value.</param>
		void _changeReverbWidth(IntPtr reverbDSPProcessor, float newWidth);

		/// <summary>
		/// This method changes the reverb freeze mode associated with a given reverb DSP SFX object.
		/// </summary>
		/// <param name="reverbDSPProcessor">Pointer reference for the reverb DSP Effect object previously allocated in memory.</param>
		/// <param name="newFreezeMode">The new freeze mode value.</param>
		void _changeReverbFreezeMode(IntPtr reverbDSPProcessor, bool newFreezeMode);

		#pragma endregion ReverbDSP
		#pragma region ChorusDSP

		/// <summary>
		/// This method changes the chorus rate associated with a given chorus DSP SFX object.
		/// </summary>
		/// <param name="chorusDSPProcessor">Pointer reference for the chorus DSP Effect object previously allocated in memory.</param>
		/// <param name="newRate">The new rate value.</param>
		void _changeChorusRate(IntPtr chorusDSPProcessor, float newRate);

		/// <summary>
		/// This method changes the chorus depth associated with a given chorus DSP SFX object.
		/// </summary>
		/// <param name="chorusDSPProcessor">Pointer reference for the chorus DSP Effect object previously allocated in memory.</param>
		/// <param name="newDepth">The new depth value.</param>
		void _changeChorusDepth(IntPtr chorusDSPProcessor, float newDepth);

		/// <summary>
		/// This method changes the chorus center delay associated with a given chorus DSP SFX object.
		/// </summary>
		/// <param name="chorusDSPProcessor">Pointer reference for the chorus DSP Effect object previously allocated in memory.</param>
		/// <param name="newCenterDelay">The new center delay value.</param>
		void _changeChorusCenterDelay(IntPtr chorusDSPProcessor, float newCenterDelay);

		/// <summary>
		/// This method changes the chorus feedback associated with a given chorus DSP SFX object.
		/// </summary>
		/// <param name="chorusDSPProcessor">Pointer reference for the chorus DSP Effect object previously allocated in memory.</param>
		/// <param name="newFeedback">The new feedback value.</param>
		void _changeChorusFeedback(IntPtr chorusDSPProcessor, float newFeedback);

		/// <summary>
		/// This method changes the chorus mix associated with a given chorus DSP SFX object.
		/// </summary>
		/// <param name="chorusDSPProcessor">Pointer reference for the chorus DSP Effect object previously allocated in memory.</param>
		/// <param name="newMix">The new mix value.</param>
		void _changeChorusMix(IntPtr chorusDSPProcessor, float newMix);

		#pragma endregion ChorusDSP
		#pragma endregion DSPLogic
	};
}