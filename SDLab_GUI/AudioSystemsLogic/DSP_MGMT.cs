using SDLab_InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic
{
    /// <summary>
    /// Provides functionality for applying and controlling a distortion DSP effect and talks with the Interop Layer
    /// </summary>
    public class DistortionDSP
    {
        private readonly AudioEngineWrapper engineBridgeRef;
        private nint distortionDSPProcessor;

        /// <summary>
        /// Specifies the types of audio distortion effects.
        /// </summary>
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

        /// <summary>
        /// Handles changes to the distortion drive slider and updates the distortion drive value accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data containing the new value.</param>
        public void distortionDriveChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            Slider originSlider = sender as Slider;
            drive = (float)originSlider.Value;
            engineBridgeRef.ChangeDistortionDrive(distortionDSPProcessor, drive);
        }

        /// <summary>
        /// Handles changes to the distortion type selection and updates the distortion function accordingly.
        /// </summary>
        /// <param name="sender">The Picker control that triggered the event.</param>
        /// <param name="e">Event data associated with the change.</param>
        public void distortionTypeChangeEvent(object? sender, EventArgs e)
        {
            Picker originPicker = sender as Picker;

            if (!Enum.TryParse((string)originPicker.SelectedItem, out distortionType)) return;

            engineBridgeRef.ChangeDistortionFunctionToUse(distortionDSPProcessor, (int)distortionType);
        }

        /// <summary>
        /// Retrieves an array of visualization sample data from the distortion DSP processor.
        /// </summary>
        /// <returns>A float array containing the visualization samples.</returns>
        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(distortionDSPProcessor, (int)Global.enumDSPType.DISTORTION);
        }
    }

    /// <summary>
    /// Provides functionality for managing and controlling a compressor DSP effect and integrates with the Interop Layer.
    /// </summary>
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

        /// <summary>
        /// Handles changes to the compressor threshold slider, updates its rotation, and applies the new threshold value to the compressor processor.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the threshold change.</param>
        public void compressorThresholdChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Rotation = 180;
            threshold = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorThreshold(compressorDSPProcessor, threshold);
        }

        /// <summary>
        /// Handles changes to the compressor ratio slider and updates the compressor ratio accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void compressorRatioChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            ratio = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorRatio(compressorDSPProcessor, ratio);
        }

        /// <summary>
        /// Handles changes to the compressor attack value by updating the internal state and notifying the engine bridge.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the change.</param>
        public void compressorAttackChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            attack = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorAttack(compressorDSPProcessor, attack);
        }

        /// <summary>
        /// Handles the event when the compressor release slider value changes and updates the compressor release parameter.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void compressorReleaseChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            release = (float)originSlider.Value;
            engineBridgeRef.ChangeCompressorRelease(compressorDSPProcessor, release);
        }

        /// <summary>
        /// Retrieves an array of visualization sample data from the compressor DSP processor.
        /// </summary>
        /// <returns>An array of floating-point values representing the visualization samples.</returns>
        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(compressorDSPProcessor, (int)Global.enumDSPType.COMPRESSOR);
        }
    }

    /// <summary>
    /// Provides control and parameter management for a reverb DSP effect within an audio engine, and talks with the Interop Layer
    /// </summary>
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

        /// <summary>
        /// Handles changes to the reverb room size slider and updates the reverb effect accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void reverbRoomSizeChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            roomSize = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbRoomSize(reverbDSPProcessor, roomSize);
        }

        /// <summary>
        /// Handles the event when the reverb damping slider value changes and updates the reverb damping parameter in the audio engine.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void reverbDampingChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            damping = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbDamping(reverbDSPProcessor, damping);
        }

        /// <summary>
        /// Handles the event triggered when the reverb wet level slider value changes and updates the reverb effect accordingly.
        /// </summary>
        /// <param name="sender">The slider control that initiated the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void reverbWetLevelChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            wetLevel = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbWetLevel(reverbDSPProcessor, wetLevel);
        }

        /// <summary>
        /// Handles the event when the reverb dry level slider value changes and updates the reverb dry level accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void reverbDryLevelChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            dryLevel = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbDryLevel(reverbDSPProcessor, dryLevel);
        }

        /// <summary>
        /// Handles changes to the reverb width slider and updates the reverb width in the audio engine.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the slider value change.</param>
        public void reverbWidthChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            width = (float)originSlider.Value;
            engineBridgeRef.ChangeReverbWidth(reverbDSPProcessor, width);
        }

        /// <summary>
        /// Handles the event triggered when the reverb freeze mode switch is toggled, updating the freeze mode state and notifying the audio engine.
        /// </summary>
        /// <param name="sender">The switch control that triggered the event.</param>
        /// <param name="e">Event data associated with the toggle action.</param>
        public void reverbFreezeModeChangeEvent(object? sender, EventArgs e)
        {
            Switch originSwitchBTN = sender as Switch;
            FreezeMode = originSwitchBTN.IsToggled;
            engineBridgeRef.ChangeReverbFreezeMode(reverbDSPProcessor, freezeMode);
        }

        /// <summary>
        /// Retrieves an array of visualization samples from the reverb DSP processor.
        /// </summary>
        /// <returns>An array of floating-point values representing the visualization samples.</returns>
        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(reverbDSPProcessor, (int)Global.enumDSPType.REVERB);
        }
    }

    /// <summary>
    /// Provides functionality to manage and control a chorus DSP audio effect, and talks with the Interop Layer
    /// </summary>
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

        /// <summary>
        /// Handles changes to the chorus rate slider and updates the chorus effect rate accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the rate change.</param>
        public void chorusRateChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            rate = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusRate(chorusDSPProcessor, rate);
        }

        /// <summary>
        /// Handles changes to the chorus depth slider and updates the chorus effect depth accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Event data associated with the change.</param>
        public void chorusDepthChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            depth = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusDepth(chorusDSPProcessor, depth);
        }

        /// <summary>
        /// Handles changes to the chorus center delay slider and updates the chorus effect center delay accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chorusCenterDelayChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            centerDelay = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusCenterDelay(chorusDSPProcessor, centerDelay);
        }

        /// <summary>
        /// Handles changes to the chorus feedback slider and updates the chorus effect feedback accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chorusFeedbackChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            feedback = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusFeedback(chorusDSPProcessor, feedback);
        }

        /// <summary>
        /// Handles changes to the chorus mix slider and updates the chorus effect mix accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chorusMixChangeEvent(object? sender, EventArgs e)
        {
            Slider originSlider = sender as Slider;
            mix = (float)originSlider.Value;
            engineBridgeRef.ChangeChorusMix(chorusDSPProcessor, mix);
        }

        /// <summary>
        /// Pushes a float sample array for DSP output sound wave visualization.
        /// </summary>
        /// <returns>A floating-point array with the sample data.</returns>
        public float[] pushVisSampleArray()
        {
            return engineBridgeRef.PushVisSamples(chorusDSPProcessor, (int)Global.enumDSPType.CHORUS);
        }
    }
}