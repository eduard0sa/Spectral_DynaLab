using NAudio.Wave;
using SDLab_GUI.UIComponents.TrackUIComponents;
using SDLab_GUI.InteropWrapper;
using static SDLab_GUI.Global;
using SDLab_GUI.Configurations;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    /// <summary>
    /// Provides audio sample data from a JUCE-based audio engine, supporting real-time processing and management of DSP effects such as distortion, compressor, reverb, and chorus.
    /// </summary>
    public abstract class JuceAudioProvider : ISampleProvider
    {
        protected nint engine;
        protected AudioEngineWrapper engineBridgeRef;
        protected float currentGain;
        protected int samplesPerBlock;

        protected List<structVariableDataTypeUnit> dspProcessors = new List<structVariableDataTypeUnit>();

        public WaveFormat WaveFormat { get; protected set; }

        public nint Engine { get => engine; }
        public float CurrentGain { get => currentGain; set => currentGain = value; }
        public List<structVariableDataTypeUnit> DspProcessors { get => dspProcessors; }

        /// <summary>
        /// Asks the core audio system to fill a sample array with float wave values, and reads it into the player
        /// </summary>
        /// <param name="buffer">The array to fill with audio sample data.</param>
        /// <param name="offset">The index in the buffer at which to begin writing samples.</param>
        /// <param name="count">The number of samples to read into the buffer.</param>
        /// <returns>The number of samples read into the buffer.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int processedSamplesCount = 0;

            while (processedSamplesCount < count)
            {
                int samplesToProcess = Math.Min(samplesPerBlock, count - processedSamplesCount);

                engineBridgeRef.EngineProcessWave(Engine, buffer, samplesToProcess, offset + processedSamplesCount);

                processedSamplesCount += samplesToProcess;
            }

            return count;
        }

        /// <summary>
        /// Sets the current gain and updates the core audio engine with the specified gain value.
        /// </summary>
        /// <param name="gain">The new gain value to set.</param>
        public void changeGain(float gain)
        {
            currentGain = gain;
            engineBridgeRef.ChangeGain(engine, gain);
        }

        /// <summary>
        /// Retrieves an array of oscillator playing samples from the core audio engine, for wave visualization.
        /// </summary>
        /// <returns>An array of float values representing oscillator visualization samples.</returns>
        public float[] pushOSCVisSampleArray()
        {
            return engineBridgeRef.PushOscVisSamples(engine);
        }

        /// <summary>
        /// Adds a DSP effect of the specified type to the processor collection and returns its data unit.
        /// </summary>
        /// <param name="dspEffectType">The type of DSP effect to add.</param>
        /// <returns>A structVariableDataTypeUnit representing the added DSP effect.</returns>
        public structVariableDataTypeUnit addDSPEffect(enumDSPType dspEffectType, EditorConfigs defaultSettings)
        {
            structVariableDataTypeUnit dspUnit = new structVariableDataTypeUnit();

            switch (dspEffectType)
            {
                case enumDSPType.DISTORTION:
                    //New DistotionDSP audio processing class instance
                    DistortionDSP newDistortionProcessor = new DistortionDSP(engine, engineBridgeRef, defaultSettings);
                    //New DistortionDSP UI component class instance
                    DSPEffectItem<DistortionDSP> newDistortionDSP = new DSPEffectItem<DistortionDSP>(this, dspEffectType, newDistortionProcessor, newDistortionProcessor.pushVisSampleArray);

                    //Adding effect parameter controls
                    newDistortionDSP.addSliderControl(0, "Drive:", enumBaseColor.RED, newDistortionProcessor.DistortionDriveSliderData, newDistortionProcessor.distortionDriveChangeEvent);
                    newDistortionDSP.addPickerControl(0, "Distortion Type:", enumBaseColor.RED, newDistortionProcessor.DistortionTypePickerData, newDistortionProcessor.distortionTypeChangeEvent);

                    //Packging effect data into a dataUnit struct instance
                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_DISTORTION_DSP_CLASS,
                        dataUnit = newDistortionDSP
                    };

                    DspProcessors.Add(dspUnit); //Adding the dspUnit to the DSP List

                    return dspUnit;

                case enumDSPType.COMPRESSOR:
                    CompressorDSP newCompressorProcessor = new CompressorDSP(engine, engineBridgeRef, defaultSettings);
                    DSPEffectItem<CompressorDSP> newCompressorDSP = new DSPEffectItem<CompressorDSP>(this, dspEffectType, newCompressorProcessor, newCompressorProcessor.pushVisSampleArray);

                    newCompressorDSP.addControlGroup();
                    newCompressorDSP.addSliderControl(0, "Threshold:", enumBaseColor.RED, newCompressorProcessor.CompressorThresholdSliderData, newCompressorProcessor.compressorThresholdChangeEvent);
                    newCompressorDSP.addSliderControl(0, "Ratio:", enumBaseColor.RED, newCompressorProcessor.CompressorRatioSliderData, newCompressorProcessor.compressorRatioChangeEvent);
                    newCompressorDSP.addSliderControl(1, "Attack:", enumBaseColor.RED, newCompressorProcessor.CompressorAttackSliderData, newCompressorProcessor.compressorAttackChangeEvent);
                    newCompressorDSP.addSliderControl(1, "Release:", enumBaseColor.RED, newCompressorProcessor.CompressorReleaseSliderData, newCompressorProcessor.compressorReleaseChangeEvent);

                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS,
                        dataUnit = newCompressorDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case enumDSPType.REVERB:
                    ReverbDSP newReverbProcessor = new ReverbDSP(engine, engineBridgeRef, defaultSettings);
                    DSPEffectItem<ReverbDSP> newReverbDSP = new DSPEffectItem<ReverbDSP>(this, dspEffectType, newReverbProcessor, newReverbProcessor.pushVisSampleArray);

                    newReverbDSP.addControlGroup();

                    newReverbDSP.addSliderControl(0, "Room Size:", enumBaseColor.RED, newReverbProcessor.ReverbRoomSizeSliderData, newReverbProcessor.reverbRoomSizeChangeEvent);
                    newReverbDSP.addSliderControl(0, "Damping:", enumBaseColor.RED, newReverbProcessor.ReverbDampingSliderData, newReverbProcessor.reverbDampingChangeEvent);
                    newReverbDSP.addSliderControl(0, "Wet Level:", enumBaseColor.RED, newReverbProcessor.ReverbWetLevelSliderData, newReverbProcessor.reverbWetLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Dry Level:", enumBaseColor.RED, newReverbProcessor.ReverbDryLevelSliderData, newReverbProcessor.reverbDryLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Width:", enumBaseColor.RED, newReverbProcessor.ReverbWidthSliderData, newReverbProcessor.reverbWidthChangeEvent);
                    newReverbDSP.addSwitchControl(1, "Freeze Mode:", enumBaseColor.RED, newReverbProcessor.ReverbFreezeModeSliderData, newReverbProcessor.reverbFreezeModeChangeEvent);

                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_REVERB_DSP_CLASS,
                        dataUnit = newReverbDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case enumDSPType.CHORUS:
                    ChorusDSP newChorusProcessor = new ChorusDSP(engine, engineBridgeRef, defaultSettings);
                    DSPEffectItem<ChorusDSP> newChorusDSP = new DSPEffectItem<ChorusDSP>(this, dspEffectType, newChorusProcessor, newChorusProcessor.pushVisSampleArray);

                    newChorusDSP.addControlGroup();

                    newChorusDSP.addSliderControl(0, "Rate:", enumBaseColor.RED, newChorusProcessor.ChorusRateSliderData, newChorusProcessor.chorusRateChangeEvent);
                    newChorusDSP.addSliderControl(0, "Depth:", enumBaseColor.RED, newChorusProcessor.ChorusDepthSliderData, newChorusProcessor.chorusDepthChangeEvent);
                    newChorusDSP.addSliderControl(0, "Center Delay:", enumBaseColor.RED, newChorusProcessor.ChorusCenterDelaySliderData, newChorusProcessor.chorusCenterDelayChangeEvent);
                    newChorusDSP.addSliderControl(1, "Feedback:", enumBaseColor.RED, newChorusProcessor.ChorusFeedbackSliderData, newChorusProcessor.chorusFeedbackChangeEvent);
                    newChorusDSP.addSliderControl(1, "Mix:", enumBaseColor.RED, newChorusProcessor.ChorusMixSliderData, newChorusProcessor.chorusMixChangeEvent);

                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_CHORUS_DSP_CLASS,
                        dataUnit = newChorusDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                default:
                    return dspUnit;
            }
        }

        /// <summary>
        /// Removes the specified DSP effect from the engine and stops its associated graph visualization update worker.
        /// </summary>
        /// <param name="effectData">The DSP effect data to be removed.</param>
        public void removeDSPEffect(structVariableDataTypeUnit effectData)
        {
            switch (effectData.dataType)
            {
                case enumVariableDataType.TYPE_DISTORTION_DSP_CLASS:
                    ((DSPEffectItem<DistortionDSP>)effectData.dataUnit).stopGraphUpdateWorker();
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<DistortionDSP>)effectData.dataUnit).DspProcessor.DistortionDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS:
                    ((DSPEffectItem<CompressorDSP>)effectData.dataUnit).stopGraphUpdateWorker();
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<CompressorDSP>)effectData.dataUnit).DspProcessor.CompressorDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_REVERB_DSP_CLASS:
                    ((DSPEffectItem<ReverbDSP>)effectData.dataUnit).stopGraphUpdateWorker();
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<ReverbDSP>)effectData.dataUnit).DspProcessor.ReverbDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_CHORUS_DSP_CLASS:
                    ((DSPEffectItem<ChorusDSP>)effectData.dataUnit).stopGraphUpdateWorker();
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<ChorusDSP>)effectData.dataUnit).DspProcessor.ChorusDSPProcessor);
                    break;
            }

            dspProcessors.Remove(effectData);
        }
    }
}