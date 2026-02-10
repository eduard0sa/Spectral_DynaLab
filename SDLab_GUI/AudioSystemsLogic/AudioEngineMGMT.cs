using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SDLab_GUI.UIComponents;
using SDLab_InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic
{
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
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
            {
                ReadFully = true
            };

            vsp = new VolumeSampleProvider(mixer);

            vsp.Volume = 1.0f;
        }

        public void initMixer()
        {
            output.Init(vsp);
        }

        public void PlayMixer()
        {
            output.Play();
        }

        public void PauseMixer()
        {
            output.Pause();
        }

        public void removeAudioEngine(JuceAudioProvider provider)
        {
            int dspProcessorsCount = (int)provider.DspProcessors.Count;

            for (int i = 0; i < provider.DspProcessors.Count; i++)
            {
                provider.removeDSPEffect(provider.DspProcessors[i]);
            }

            /*System.Timers.Timer counter = new System.Timers.Timer();

            counter.Interval = 33 * dspProcessorsCount;

            counter.Elapsed += delegate
            {
                counter.Stop();*/
            mixer.RemoveMixerInput(provider);
            oscillators.Remove(provider);
            _AudioEngineRef.DestroyEngine(provider.Engine);
            /*};

            counter.Start();*/
        }

        public JuceAudioProvider LaunchAudioEngine()
        {
            AudioEngineOsc = _AudioEngineRef.CreateEngine();
            JuceAudioProvider provider = new JuceAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

            provider.changeFrequency(defaultFrequency);
            provider.changeGain(defaultGain);

            mixer.AddMixerInput(provider);

            oscillators.Add(provider);
            return provider;
        }
    }

    public class JuceAudioProvider : ISampleProvider
    {
        private readonly nint engine;
        private readonly AudioEngineWrapper engineBridgeRef;
        private float currentFrequency;
        private float currentGain;
        private int samplesPerBlock;

        private List<structVariableDataTypeUnit> dspProcessors = new List<structVariableDataTypeUnit>();

        public WaveFormat WaveFormat { get; }

        public nint Engine { get => engine; }
        public float CurrentFrequency { get => currentFrequency; }
        public float CurrentGain { get => currentGain; }
        public List<structVariableDataTypeUnit> DspProcessors { get => dspProcessors; }

        public JuceAudioProvider(nint engine, AudioEngineWrapper engineBridgeRef, int sampleRate, int channels)
        {
            samplesPerBlock = 512;
            this.engine = engine;
            this.engineBridgeRef = engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            engineBridgeRef.EnginePrepareToPlay(engine, sampleRate, samplesPerBlock);
        }

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

        public void changeFrequency(float frequency)
        {
            currentFrequency = frequency;
            engineBridgeRef.ChangeFrequency(engine, frequency);
        }

        public void changeGain(float frequency)
        {
            currentGain = frequency;
            engineBridgeRef.ChangeGain(engine, frequency);
        }

        public structVariableDataTypeUnit addDSPEffect(enumDSPType dspEffectType)
        {
            structVariableDataTypeUnit dspUnit = new structVariableDataTypeUnit();

            switch (dspEffectType)
            {
                case enumDSPType.DISTORTION:
                    DistortionDSP newDistortionProcessor = new DistortionDSP(engine, engineBridgeRef);
                    DSPEffectItem<DistortionDSP> newDistortionDSP = new DSPEffectItem<DistortionDSP>(this, dspEffectType, newDistortionProcessor, newDistortionProcessor.pushVisSampleArray);

                    newDistortionDSP.addSliderControl(0, "Drive:", newDistortionProcessor.DistortionDriveSliderData, newDistortionProcessor.distortionDriveChangeEvent);
                    newDistortionDSP.addPickerControl(0, "Distortion Type:", newDistortionProcessor.DistortionTypePickerData, newDistortionProcessor.distortionTypeChangeEvent);

                    dspUnit = new structVariableDataTypeUnit()
                    {
                        dataType = enumVariableDataType.TYPE_DISTORTION_DSP_CLASS,
                        dataUnit = newDistortionDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case enumDSPType.COMPRESSOR:
                    CompressorDSP newCompressorProcessor = new CompressorDSP(engine, engineBridgeRef);
                    DSPEffectItem<CompressorDSP> newCompressorDSP = new DSPEffectItem<CompressorDSP>(this, dspEffectType, newCompressorProcessor, newCompressorProcessor.pushVisSampleArray);

                    newCompressorDSP.addControlGroup();
                    newCompressorDSP.addSliderControl(0, "Threshold:", newCompressorProcessor.CompressorThresholdSliderData, newCompressorProcessor.compressorThresholdChangeEvent);
                    newCompressorDSP.addSliderControl(0, "Ratio:", newCompressorProcessor.CompressorRatioSliderData, newCompressorProcessor.compressorRatioChangeEvent);
                    newCompressorDSP.addSliderControl(1, "Attack:", newCompressorProcessor.CompressorAttackSliderData, newCompressorProcessor.compressorAttackChangeEvent);
                    newCompressorDSP.addSliderControl(1, "Release:", newCompressorProcessor.CompressorReleaseSliderData, newCompressorProcessor.compressorReleaseChangeEvent);

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

                    newReverbDSP.addSliderControl(0, "Room Size:", newReverbProcessor.ReverbRoomSizeSliderData, newReverbProcessor.reverbRoomSizeChangeEvent);
                    newReverbDSP.addSliderControl(0, "Damping:", newReverbProcessor.ReverbDampingSliderData, newReverbProcessor.reverbDampingChangeEvent);
                    newReverbDSP.addSliderControl(0, "Wet Level:", newReverbProcessor.ReverbWetLevelSliderData, newReverbProcessor.reverbWetLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Dry Level:", newReverbProcessor.ReverbDryLevelSliderData, newReverbProcessor.reverbDryLevelChangeEvent);
                    newReverbDSP.addSliderControl(1, "Width:", newReverbProcessor.ReverbWidthSliderData, newReverbProcessor.reverbWidthChangeEvent);
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

                    newChorusDSP.addSliderControl(0, "Rate:", newChorusProcessor.ChorusRateSliderData, newChorusProcessor.chorusRateChangeEvent);
                    newChorusDSP.addSliderControl(0, "Depth:", newChorusProcessor.ChorusDepthSliderData, newChorusProcessor.chorusDepthChangeEvent);
                    newChorusDSP.addSliderControl(0, "Center Delay:", newChorusProcessor.ChorusCenterDelaySliderData, newChorusProcessor.chorusCenterDelayChangeEvent);
                    newChorusDSP.addSliderControl(1, "Feedback:", newChorusProcessor.ChorusFeedbackSliderData, newChorusProcessor.chorusFeedbackChangeEvent);
                    newChorusDSP.addSliderControl(1, "Mix:", newChorusProcessor.ChorusMixSliderData, newChorusProcessor.chorusMixChangeEvent);

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

        public void removeDSPEffect(structVariableDataTypeUnit effectData)
        {
            switch (effectData.dataType)
            {
                case enumVariableDataType.TYPE_DISTORTION_DSP_CLASS:
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<DistortionDSP>)effectData.dataUnit).DspProcessor.DistortionDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS:
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<CompressorDSP>)effectData.dataUnit).DspProcessor.CompressorDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_REVERB_DSP_CLASS:
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<ReverbDSP>)effectData.dataUnit).DspProcessor.ReverbDSPProcessor);
                    break;
                case enumVariableDataType.TYPE_CHORUS_DSP_CLASS:
                    engineBridgeRef.RemoveDSPEffect(engine, ((DSPEffectItem<ChorusDSP>)effectData.dataUnit).DspProcessor.ChorusDSPProcessor);
                    break;
            }

            dspProcessors.Remove(effectData);
        }
    }
}