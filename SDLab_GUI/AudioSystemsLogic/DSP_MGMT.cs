using SDLab_InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic
{
    public class DistortionDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private nint distortionDSPProcessor;

        enum enum_distortionType
        {
            SoftClip,
            HardClip,
            Foldback
        }

        private float drive = 2;
        private enum_distortionType distortionType = enum_distortionType.SoftClip;
        private structSliderData distortionDriveSliderData = new structSliderData()
        {
            minVal = 1.0f,
            maxVal = 500.0f,
            defVal = 250f,
            numDisplayDecPlaces = 2
        };
        private structPickerData distortionTypePickerData = new structPickerData()
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
        internal structSliderData DistortionDriveSliderData { get => distortionDriveSliderData; set => distortionDriveSliderData = value; }
        internal structPickerData DistortionTypePickerData { get => distortionTypePickerData; set => distortionTypePickerData = value; }
        public nint DistortionDSPProcessor { get => distortionDSPProcessor; }

        public DistortionDSP(nint engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            distortionDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)enumDSPType.DISTORTION);
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

            if (!Enum.TryParse((string)originPicker.SelectedItem, out distortionType)) return;

            engineBridgeRef.ChangeDistortionFunctionToUse(distortionDSPProcessor, (int)distortionType);
        }

        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(distortionDSPProcessor, (int)Global.enumDSPType.DISTORTION);
        }
    }

    public class CompressorDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private nint compressorDSPProcessor;

        private float threshold = 2;
        private float ratio = 2;
        private float attack = 2;
        private float release = 2;

        private structSliderData compressorThresholdSliderData = new structSliderData()
        {
            minVal = -30f,
            maxVal = 30f,
            defVal = -10f,
            numDisplayDecPlaces = 2
        };

        private structSliderData compressorRatioSliderData = new structSliderData()
        {
            minVal = 1f,
            maxVal = 20f,
            defVal = 2.0f,
            numDisplayDecPlaces = 2
        };

        private structSliderData compressorAttackSliderData = new structSliderData()
        {
            minVal = 0.01f,
            maxVal = 200f,
            defVal = 20f,
            numDisplayDecPlaces = 2
        };

        private structSliderData compressorReleaseSliderData = new structSliderData()
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

        internal structSliderData CompressorThresholdSliderData { get => compressorThresholdSliderData; set => compressorThresholdSliderData = value; }
        internal structSliderData CompressorRatioSliderData { get => compressorRatioSliderData; set => compressorRatioSliderData = value; }
        internal structSliderData CompressorAttackSliderData { get => compressorAttackSliderData; set => compressorAttackSliderData = value; }
        internal structSliderData CompressorReleaseSliderData { get => compressorReleaseSliderData; set => compressorReleaseSliderData = value; }

        public nint CompressorDSPProcessor { get => compressorDSPProcessor; }

        public CompressorDSP(nint engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            compressorDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)enumDSPType.COMPRESSOR);
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

        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(compressorDSPProcessor, (int)Global.enumDSPType.COMPRESSOR);
        }
    }

    public class ReverbDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private nint reverbDSPProcessor;

        private float roomSize = 0.5f;
        private float damping = 0.5f;
        private float wetLevel = 0.5f;
        private float dryLevel = 1.0f;
        private float width = 0.5f;
        private bool freezeMode = false;

        private structSliderData reverbRoomSizeSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private structSliderData reverbDampingSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private structSliderData reverbWetLevelSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private structSliderData reverbDryLevelSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 1.0f,
            numDisplayDecPlaces = 2
        };

        private structSliderData reverbWidthSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        private structSliderData reverbFreezeModeSliderData = new structSliderData()
        {
            minVal = 0f,
            maxVal = 1f,
            defVal = 0.5f,
            numDisplayDecPlaces = 0
        };


        public nint ReverbDSPProcessor { get => reverbDSPProcessor; }
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

        public ReverbDSP(nint engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            reverbDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)enumDSPType.REVERB);
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

        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(reverbDSPProcessor, (int)Global.enumDSPType.REVERB);
        }
    }

    public class ChorusDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private nint chorusDSPProcessor;

        private float rate = 0.8f;
        private float depth = 0.4f;
        private float centerDelay = 25.0f;
        private float feedback = 0.0f;
        private float mix = 0.5f;

        private structSliderData chorusRateSliderData = new structSliderData()
        {
            minVal = 0.1f,
            maxVal = 3.0f,
            defVal = 0.8f,
            numDisplayDecPlaces = 2
        };

        private structSliderData chorusDepthSliderData = new structSliderData()
        {
            minVal = 0.1f,
            maxVal = 1.0f,
            defVal = 0.4f,
            numDisplayDecPlaces = 2
        };

        private structSliderData chorusCenterDelaySliderData = new structSliderData()
        {
            minVal = 0.1f,
            maxVal = 60f,
            defVal = 25.0f,
            numDisplayDecPlaces = 2
        };

        private structSliderData chorusFeedbackSliderData = new structSliderData()
        {
            minVal = 0.0f,
            maxVal = 0.4f,
            defVal = 0.0f,
            numDisplayDecPlaces = 2
        };

        private structSliderData chorusMixSliderData = new structSliderData()
        {
            minVal = 0.3f,
            maxVal = 1.0f,
            defVal = 0.5f,
            numDisplayDecPlaces = 2
        };

        public nint ChorusDSPProcessor { get => chorusDSPProcessor; }

        public float Rate { get => rate; set => rate = value; }
        public float Depth { get => depth; set => depth = value; }
        public float CenterDelay { get => centerDelay; set => centerDelay = value; }
        public float Feedback { get => feedback; set => feedback = value; }
        public float Mix { get => mix; set => mix = value; }

        internal structSliderData ChorusRateSliderData { get => chorusRateSliderData; set => chorusRateSliderData = value; }
        public structSliderData ChorusDepthSliderData { get => chorusDepthSliderData; set => chorusDepthSliderData = value; }
        public structSliderData ChorusCenterDelaySliderData { get => chorusCenterDelaySliderData; set => chorusCenterDelaySliderData = value; }
        public structSliderData ChorusFeedbackSliderData { get => chorusFeedbackSliderData; set => chorusFeedbackSliderData = value; }
        public structSliderData ChorusMixSliderData { get => chorusMixSliderData; set => chorusMixSliderData = value; }

        public ChorusDSP(nint engine, AudioEngineWrapper engineBridgeRef)
        {
            this.engineBridgeRef = engineBridgeRef;
            chorusDSPProcessor = engineBridgeRef.AddDSPEffect(engine, (int)enumDSPType.CHORUS);
        }

        public void chorusRateChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            rate = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusRate(chorusDSPProcessor, rate);
        }

        public void chorusDepthChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            depth = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusDepth(chorusDSPProcessor, depth);
        }

        public void chorusCenterDelayChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            centerDelay = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusCenterDelay(chorusDSPProcessor, centerDelay);
        }

        public void chorusFeedbackChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            feedback = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusFeedback(chorusDSPProcessor, feedback);
        }

        public void chorusMixChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            mix = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusMix(chorusDSPProcessor, mix);
        }

        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(chorusDSPProcessor, (int)Global.enumDSPType.CHORUS);
        }
    }
}