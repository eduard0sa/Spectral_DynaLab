using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SDLab_InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI
{
    public class AudioEngineMGMT
    {
        private float defaultFrequency = 38.0f;
        private float defaultGain = 0.5f;

        public AudioEngineWrapper _AudioEngineRef = new AudioEngineWrapper();
        public IntPtr AudioEngineOsc;
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
            for(int i = 0; i < provider.DspProcessors.Count; i++)
            {
                provider.removeDSPEffect(provider.DspProcessors[i]);
            }

            mixer.RemoveMixerInput(provider);
            oscillators.Remove(provider);
            _AudioEngineRef.DestroyEngine(provider.Engine);
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
        private readonly IntPtr engine;
        private readonly AudioEngineWrapper engineBridgeRef;
        private float currentFrequency;
        private float currentGain;

        private List<Global.structVariableDataTypeUnit> dspProcessors = new List<Global.structVariableDataTypeUnit>();

        public WaveFormat WaveFormat { get; }

        public IntPtr Engine { get => engine; }
        public float CurrentFrequency { get => currentFrequency; }
        public float CurrentGain { get => currentGain; }
        public List<structVariableDataTypeUnit> DspProcessors { get => dspProcessors; }

        public JuceAudioProvider(IntPtr engine, AudioEngineWrapper engineBridgeRef, int sampleRate, int channels)
        {
            this.engine = engine;
            this.engineBridgeRef = engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            engineBridgeRef.EnginePrepareToPlay(engine, sampleRate, 512);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            engineBridgeRef.EngineProcessWave(Engine, buffer, count, offset);
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

        public structVariableDataTypeUnit addDSPEffect(Global.enumDSPType dspEffectType)
        {
            Global.structVariableDataTypeUnit dspUnit = new structVariableDataTypeUnit();

            switch (dspEffectType)
            {
                case Global.enumDSPType.DISTORTION:
                    DistortionDSP newDistortionProcessor = new DistortionDSP(engine, engineBridgeRef);
                    DSPEffectItem<DistortionDSP> newDistortionDSP = new DSPEffectItem<DistortionDSP>(this, dspEffectType, newDistortionProcessor);

                    newDistortionDSP.addSliderControl("Drive:", newDistortionProcessor.DistortionDriveSliderData, newDistortionProcessor.distortionDriveChangeEvent);
                    newDistortionDSP.addPickerControl("Distortion Type:", newDistortionProcessor.DistortionTypePickerData, newDistortionProcessor.distortionTypeChangeEvent);

                    dspUnit = new Global.structVariableDataTypeUnit()
                    {
                        dataType = Global.enumVariableDataType.TYPE_DISTORTION_DSP_CLASS,
                        dataUnit = newDistortionDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case Global.enumDSPType.COMPRESSOR:
                    CompressorDSP newCompressorProcessor = new CompressorDSP(engine, engineBridgeRef);
                    DSPEffectItem<CompressorDSP> newCompressorDSP = new DSPEffectItem<CompressorDSP>(this, dspEffectType, newCompressorProcessor);

                    newCompressorDSP.addSliderControl("Threshold:", newCompressorProcessor.CompressorThresholdSliderData, newCompressorProcessor.compressorThresholdChangeEvent);
                    newCompressorDSP.addSliderControl("Ratio:", newCompressorProcessor.CompressorRatioSliderData, newCompressorProcessor.compressorRatioChangeEvent);
                    newCompressorDSP.addSliderControl("Attack:", newCompressorProcessor.CompressorAttackSliderData, newCompressorProcessor.compressorAttackChangeEvent);
                    newCompressorDSP.addSliderControl("Release:", newCompressorProcessor.CompressorReleaseSliderData, newCompressorProcessor.compressorReleaseChangeEvent);

                    dspUnit = new Global.structVariableDataTypeUnit()
                    {
                        dataType = Global.enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS,
                        dataUnit = newCompressorDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                case Global.enumDSPType.REVERB:
                    ReverbDSP newReverbProcessor = new ReverbDSP(engine, engineBridgeRef);
                    DSPEffectItem<ReverbDSP> newReverbDSP = new DSPEffectItem<ReverbDSP>(this, dspEffectType, newReverbProcessor);

                    newReverbDSP.addSliderControl("Room Size:", newReverbProcessor.ReverbRoomSizeSliderData, newReverbProcessor.reverbRoomSizeChangeEvent);
                    newReverbDSP.addSliderControl("Damping:", newReverbProcessor.ReverbDampingSliderData, newReverbProcessor.reverbDampingChangeEvent);
                    newReverbDSP.addSliderControl("Wet Level:", newReverbProcessor.ReverbWetLevelSliderData, newReverbProcessor.reverbWetLevelChangeEvent);
                    newReverbDSP.addSliderControl("Dry Level:", newReverbProcessor.ReverbDryLevelSliderData, newReverbProcessor.reverbDryLevelChangeEvent);
                    newReverbDSP.addSliderControl("Width:", newReverbProcessor.ReverbWidthSliderData, newReverbProcessor.reverbWidthChangeEvent);
                    newReverbDSP.addSwitchControl("Freeze Mode:", newReverbProcessor.ReverbFreezeModeSliderData, newReverbProcessor.reverbFreezeModeChangeEvent);

                    dspUnit = new Global.structVariableDataTypeUnit()
                    {
                        dataType = Global.enumVariableDataType.TYPE_REVERB_DSP_CLASS,
                        dataUnit = newReverbDSP
                    };

                    DspProcessors.Add(dspUnit);

                    return dspUnit;

                default:
                    return dspUnit;
            }
        }

        public void removeDSPEffect(Global.structVariableDataTypeUnit effectData)
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
            }

            dspProcessors.Remove(effectData);
        }
    }

    public class DistortionDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private IntPtr distortionDSPProcessor;

        enum enum_distortionType
        {
            SoftClip,
            HardClip,
            Foldback
        }

        private float drive = 2;
        private enum_distortionType distortionType = enum_distortionType.SoftClip;
        private Global.structSliderData distortionDriveSliderData = new Global.structSliderData()
        {
            minVal = 1.0f,
            maxVal = 500.0f,
            defVal = 250f,
            numDisplayDecPlaces = 2
        };
        private Global.structPickerData distortionTypePickerData = new Global.structPickerData()
        {
            defValIndex = 0,
            items = new List<string>() {
                "Soft Clip",
                "Hard Clip",
                "Foldback"
            }
        };

        public float Drive { get => drive; set => drive = value; }
        private enum_distortionType DistortionType { get => distortionType; set => distortionType = value; }
        internal Global.structSliderData DistortionDriveSliderData { get => distortionDriveSliderData; set => distortionDriveSliderData = value; }
        internal Global.structPickerData DistortionTypePickerData { get => distortionTypePickerData; set => distortionTypePickerData = value; }
        public IntPtr DistortionDSPProcessor { get => distortionDSPProcessor; }

        public DistortionDSP(IntPtr engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            distortionDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)Global.enumDSPType.DISTORTION);
        }

        public void distortionDriveChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            Slider originSlider = sender as Slider;
            drive = (float)originSlider.Value;
            engineBridgeRef.ChangeDistortionDrive(distortionDSPProcessor, drive);
        }

        public void distortionTypeChangeEvent(object? sender, EventArgs e)
        {
            Picker originPicker = sender as Picker;

            if(!Enum.TryParse<enum_distortionType>((string)originPicker.SelectedItem, out distortionType)) return;

            engineBridgeRef.ChangeDistortionFunctionToUse(distortionDSPProcessor, (int)distortionType);
        }
    }

    public class CompressorDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private IntPtr compressorDSPProcessor;

        private float threshold = 2;
        private float ratio = 2;
        private float attack = 2;
        private float release = 2;

        private Global.structSliderData compressorThresholdSliderData = new Global.structSliderData()
        {
            minVal = -30f,
            maxVal = 30f,
            defVal = -10f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData compressorRatioSliderData = new Global.structSliderData()
        {
            minVal = 1f,
            maxVal = 20f,
            defVal = 2.0f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData compressorAttackSliderData = new Global.structSliderData()
        {
            minVal = 0.01f,
            maxVal = 200f,
            defVal = 20f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData compressorReleaseSliderData = new Global.structSliderData()
        {
            minVal = 10f,
            maxVal = 5000f,
            defVal = 500f,
            numDisplayDecPlaces = 2
        };

        public float Threshold { get => threshold; set => threshold = value; }
        public float Ratio { get => ratio; set => ratio = value; }
        public float Attack { get => attack; set => attack = value; }
        public float Release { get => release; set => release = value; }

        internal Global.structSliderData CompressorThresholdSliderData { get => compressorThresholdSliderData; set => compressorThresholdSliderData = value; }
        internal Global.structSliderData CompressorRatioSliderData { get => compressorRatioSliderData; set => compressorRatioSliderData = value; }
        internal Global.structSliderData CompressorAttackSliderData { get => compressorAttackSliderData; set => compressorAttackSliderData = value; }
        internal Global.structSliderData CompressorReleaseSliderData { get => compressorReleaseSliderData; set => compressorReleaseSliderData = value; }

        public IntPtr CompressorDSPProcessor { get => compressorDSPProcessor; }

        public CompressorDSP(IntPtr engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            compressorDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)Global.enumDSPType.COMPRESSOR);
        }

        public void compressorThresholdChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Rotation = 180;
            threshold = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorThreshold(compressorDSPProcessor, threshold);
        }

        public void compressorRatioChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            ratio = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorRatio(compressorDSPProcessor, ratio);
        }

        public void compressorAttackChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            attack = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorAttack(compressorDSPProcessor, attack);
        }

        public void compressorReleaseChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            release = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorRelease(compressorDSPProcessor, release);
        }
    }

    public class ReverbDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private IntPtr reverbDSPProcessor;

        private float roomSize = 0.5f;
        private float damping = 0.5f;
        private float wetLevel = 0.5f;
        private float dryLevel = 1.0f;
        private float width = 0.5f;
        private bool freezeMode = false;

        private Global.structSliderData reverbRoomSizeSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData reverbDampingSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData reverbWetLevelSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData reverbDryLevelSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 1.0f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData reverbWidthSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private Global.structSliderData reverbFreezeModeSliderData = new Global.structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 0
        };


        public IntPtr ReverbDSPProcessor { get => reverbDSPProcessor; }
        public float RoomSize { get => roomSize; set => roomSize = value; }
        public float Damping { get => damping; set => damping = value; }
        public float WetLevel { get => wetLevel; set => wetLevel = value; }
        public float DryLevel { get => dryLevel; set => dryLevel = value; }
        public float Width { get => width; set => width = value; }
        public bool FreezeMode { get => freezeMode; set => freezeMode = value; }

        internal structSliderData ReverbRoomSizeSliderData { get => reverbRoomSizeSliderData; set => reverbRoomSizeSliderData = value; }
        internal structSliderData ReverbDampingSliderData { get => reverbDampingSliderData; set => reverbDampingSliderData = value; }
        internal structSliderData ReverbWetLevelSliderData { get => reverbWetLevelSliderData; set => reverbWetLevelSliderData = value; }
        internal structSliderData ReverbDryLevelSliderData { get => reverbDryLevelSliderData; set => reverbDryLevelSliderData = value; }
        internal structSliderData ReverbWidthSliderData { get => reverbWidthSliderData; set => reverbWidthSliderData = value; }
        internal structSliderData ReverbFreezeModeSliderData { get => reverbFreezeModeSliderData; set => reverbFreezeModeSliderData = value; }

        public ReverbDSP(IntPtr engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            reverbDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)Global.enumDSPType.REVERB);
        }

        public void reverbRoomSizeChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            roomSize = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbRoomSize(reverbDSPProcessor, roomSize);
        }

        public void reverbDampingChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            damping = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbDamping(reverbDSPProcessor, damping);
        }

        public void reverbWetLevelChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            wetLevel = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbWetLevel(reverbDSPProcessor, wetLevel);
        }

        public void reverbDryLevelChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            dryLevel = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbDryLevel(reverbDSPProcessor, dryLevel);
        }

        public void reverbWidthChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            width = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbWidth(reverbDSPProcessor, width);
        }

        public void reverbFreezeModeChangeEvent(object? sender, EventArgs e)
        {
            Switch originSwitchBTN = sender as Switch;
            FreezeMode = originSwitchBTN.IsToggled;
            engineBridgeRef.ChangeReverbFreezeMode(reverbDSPProcessor, freezeMode);
        }
    }
}