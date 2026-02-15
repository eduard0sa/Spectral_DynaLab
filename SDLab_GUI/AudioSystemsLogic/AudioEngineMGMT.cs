using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SDLab_GUI.UIComponents.TrackUIComponents;
using SDLab_InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic
{
    /// <summary>
    /// <para> Audio engine management class. </para>
    /// <para> It handles sound entries and effects and contacts core audio engine through the Interop Layer. </para>
    /// </summary>
    public class AudioEngineMGMT
    {
        private float defaultFrequency = 38.0f;
        private float defaultGain = 0.5f;

        public AudioEngineWrapper _AudioEngineRef = new AudioEngineWrapper();
        public nint AudioEngineOsc;
        public List<JuceAudioProvider> oscillators = new List<JuceAudioProvider>();
        public MixingSampleProvider mixer;
        public VolumeSampleProvider vsp;
        public WaveOutEvent output = new WaveOutEvent();

        public AudioEngineMGMT()
        {
            //Initializing Mixer Provider
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
            {
                ReadFully = true
            };

            //Initializing Volume Sample Provider
            vsp = new VolumeSampleProvider(mixer);
            vsp.Volume = 1.0f;
        }

        /// <summary>
        /// Initializes the audio mixer with the specified audio source.
        /// </summary>
        public void initMixer()
        {
            output.Init(vsp);
        }

        /// <summary>
        /// Starts playback of the audio mixer output.
        /// </summary>
        public void PlayMixer()
        {
            output.Play();
        }

        /// <summary>
        /// Pauses audio playback on the mixer output.
        /// </summary>
        public void PauseMixer()
        {
            output.Pause();
        }

        /// <summary>
        /// Removes a specified audio engine and all its associated DSP effects from the app and from audio engines.
        /// </summary>
        /// <param name="provider">The JuceAudioProvider instance representing the audio engine to remove.</param>
        public void removeAudioEngine(JuceAudioProvider provider)
        {
            //Associated DSPs removal
            int dspProcessorsCount = (int)provider.DspProcessors.Count;
            int counter = 0;

            while(counter < dspProcessorsCount)
            {
                provider.removeDSPEffect(provider.DspProcessors[0]);
                counter++;
            }

            //Oscillator removal
            mixer.RemoveMixerInput(provider);
            oscillators.Remove(provider);
            _AudioEngineRef.DestroyEngine(provider.Engine);
        }

        /// <summary>
        /// Initializes a new JuceAudioProvider with default frequency and gain, adds it to the mixer and oscillator list, and returns the provider.
        /// </summary>
        /// <returns>The newly created and configured JuceAudioProvider instance.</returns>
        public JuceAudioProvider LaunchAudioEngine()
        {
            //Oscillator instantiation in the core audio engine
            AudioEngineOsc = _AudioEngineRef.CreateEngine();
            JuceAudioProvider provider = new JuceAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

            provider.changeFrequency(defaultFrequency);
            provider.changeGain(defaultGain);

            //New oscillator instance append in mixer
            mixer.AddMixerInput(provider);

            oscillators.Add(provider);
            return provider;
        }
    }

    /// <summary>
    /// Provides audio sample data from a JUCE-based audio engine, supporting real-time processing and management of DSP effects such as distortion, compressor, reverb, and chorus.
    /// </summary>
    public class JuceAudioProvider : ISampleProvider
    {
        private readonly nint engine;
        private readonly AudioEngineWrapper engineBridgeRef;
        private float currentFrequency;
        private float currentGain;
        private enumWaveShapeType waveShapeType = enumWaveShapeType.Sine;
        private int samplesPerBlock;

        private List<structVariableDataTypeUnit> dspProcessors = new List<structVariableDataTypeUnit>();

        public WaveFormat WaveFormat { get; }

        public nint Engine { get => engine; }
        public float CurrentFrequency { get => currentFrequency; }
        public float CurrentGain { get => currentGain; }
        public enumWaveShapeType WaveShapeType { get => waveShapeType; }
        public List<structVariableDataTypeUnit> DspProcessors { get => dspProcessors; }

        public JuceAudioProvider(nint engine, AudioEngineWrapper engineBridgeRef, int sampleRate, int channels)
        {
            samplesPerBlock = 512;
            this.engine = engine;
            this.engineBridgeRef = engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            engineBridgeRef.EnginePrepareToPlay(engine, sampleRate, samplesPerBlock);
        }

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

            while(processedSamplesCount < count)
            {
                int samplesToProcess = Math.Min(samplesPerBlock, count - processedSamplesCount);

                engineBridgeRef.EngineProcessWave(Engine, buffer, samplesToProcess, offset + processedSamplesCount);

                processedSamplesCount += samplesToProcess;
            }
            
            return count;
        }

        /// <summary>
        /// Sets the current frequency and updates the core audio engine with the specified frequency value.
        /// </summary>
        /// <param name="frequency">The new frequency value to set.</param>
        public void changeFrequency(float frequency)
        {
            currentFrequency = frequency;
            engineBridgeRef.ChangeFrequency(engine, frequency);
        }

        /// <summary>
        /// Sets the current gain and updates the core audio engine with the specified gain value.
        /// </summary>
        /// <param name="frequency">The new gain value to set.</param>
        public void changeGain(float gain)
        {
            currentGain = gain;
            engineBridgeRef.ChangeGain(engine, gain);
        }

        /// <summary>
        /// Sets the current waveShapeType and updates the core audio engine with the specified wave shape function.
        /// </summary>
        /// <param name="_waveShapeType"></param>
        public void changeWaveShapeFunction(enumWaveShapeType _waveShapeType)
        {
            waveShapeType = _waveShapeType;
            engineBridgeRef.ChangeWaveShapeFunction(engine, (int)_waveShapeType);
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
        public structVariableDataTypeUnit addDSPEffect(enumDSPType dspEffectType)
        {
            structVariableDataTypeUnit dspUnit = new structVariableDataTypeUnit();

            switch (dspEffectType)
            {
                case enumDSPType.DISTORTION:
                    //New DistotionDSP audio processing class instance
                    DistortionDSP newDistortionProcessor = new DistortionDSP(engine, engineBridgeRef);
                    //New DistortionDSP UI component class instance
                    DSPEffectItem<DistortionDSP> newDistortionDSP = new DSPEffectItem<DistortionDSP>(this, dspEffectType, newDistortionProcessor, newDistortionProcessor.pushVisSampleArray);

                    //Adding effect parameter controls
                    newDistortionDSP.addSliderControl(0, "Drive:", enumBaseColor.RED, newDistortionProcessor.DistortionDriveSliderData, newDistortionProcessor.distortionDriveChangeEvent);
                    newDistortionDSP.addPickerControl(0, "Distortion Type:", newDistortionProcessor.DistortionTypePickerData, newDistortionProcessor.distortionTypeChangeEvent);

                    //Packging effect data into a dataUnit struct instance
                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_DISTORTION_DSP_CLASS,
                        dataUnit = newDistortionDSP
                    };

                    DspProcessors.Add(dspUnit); //Adding the dspUnit to the DSP List

                    return dspUnit;

                case enumDSPType.COMPRESSOR:
                    CompressorDSP newCompressorProcessor = new CompressorDSP(engine, engineBridgeRef);
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
                    ReverbDSP newReverbProcessor = new ReverbDSP(engine, engineBridgeRef);
                    DSPEffectItem<ReverbDSP> newReverbDSP = new DSPEffectItem<ReverbDSP>(this, dspEffectType, newReverbProcessor, newReverbProcessor.pushVisSampleArray);

                    newReverbDSP.addControlGroup();

                    newReverbDSP.addSliderControl(0, "Room Size:", enumBaseColor.RED, newReverbProcessor.ReverbRoomSizeSliderData, newReverbProcessor.reverbRoomSizeChangeEvent);
                    newReverbDSP.addSliderControl(0, "Damping:", enumBaseColor.RED, newReverbProcessor.ReverbDampingSliderData, newReverbProcessor.reverbDampingChangeEvent);
                    newReverbDSP.addSliderControl(0, "Wet Level:", enumBaseColor.RED, newReverbProcessor.ReverbWetLevelSliderData, newReverbProcessor.reverbWetLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Dry Level:", enumBaseColor.RED, newReverbProcessor.ReverbDryLevelSliderData, newReverbProcessor.reverbDryLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Width:", enumBaseColor.RED, newReverbProcessor.ReverbWidthSliderData, newReverbProcessor.reverbWidthChangeEvent);
                    newReverbDSP.addSwitchControl(1, "Freeze Mode:", newReverbProcessor.ReverbFreezeModeSliderData, newReverbProcessor.reverbFreezeModeChangeEvent);

                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_REVERB_DSP_CLASS,
                        dataUnit = newReverbDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case enumDSPType.CHORUS:
                    ChorusDSP newChorusProcessor = new ChorusDSP(engine, engineBridgeRef);
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