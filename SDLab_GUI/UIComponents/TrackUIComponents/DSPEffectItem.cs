using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    /// <summary>
    /// DSP effect UI component class
    /// <para>It connects the UI controls and sound systems in order to enable DSP control by the user.</para>
    /// </summary>
    /// <typeparam name="T">The DSPEffect SFX Type class (eg:.DistortionDSP, CompressorDSP)</typeparam>
    internal class DSPEffectItem<T> : TrackItem
    {
        T dspProcessor;
        Global.enumDSPType dSPType;

        /// <summary>
        /// Reference to audio system's DSPProcessor instance
        /// </summary>
        public T DspProcessor { get => dspProcessor; }

        /// <summary>
        /// DSPEffectItem<T> class constructor.
        /// </summary>
        /// <param name="audioProvider">Reference of audio system's audio provider associated to DSP effect parent oscillator.</param>
        /// <param name="_dspType">The kind of DSP Processor (eg:. DISTORTION, COMPRESSOR, etc...).</param>
        /// <param name="_dspProcessor">Reference to audio system's DSP Processor engine.</param>
        /// <param name="_graphUpdateMehod">The method that is called when the resulting wave visualizer graph is updated.</param>
        public DSPEffectItem(JuceAudioProvider audioProvider, Global.enumDSPType _dspType, T _dspProcessor, Func<float[]> _graphUpdateMehod)
        {
            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;

            switch (_dspType)
            {
                case Global.enumDSPType.DISTORTION:
                    TrackItemHeader = new TrackItemHeader(this, _dspType.ToString(), new List<string>() { "RedTitle" });
                    break;
                case Global.enumDSPType.COMPRESSOR:
                    TrackItemHeader = new TrackItemHeader(this, _dspType.ToString(), new List<string>() { "YellowTitle" });
                    break;
                case Global.enumDSPType.REVERB:
                    TrackItemHeader = new TrackItemHeader(this, _dspType.ToString(), new List<string>() { "GreenTitle" });
                    break;
                case Global.enumDSPType.CHORUS:
                    TrackItemHeader = new TrackItemHeader(this, _dspType.ToString(), new List<string>() { "YellowTitle" });
                    break;
            }
            
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, _graphUpdateMehod, audioProvider);

            trackAudioProvider = audioProvider;
            dspProcessor = _dspProcessor;
            dSPType = _dspType;

            UIPaint();
        }

        /// <summary>
        /// UI sub-components painting method
        /// </summary>
        protected override void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);


            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);

            for(int i = 0; i < TrackItemControls.Count; i++)
            {
                this.SetGrow(TrackItemControls[i], 0.55f / TrackItemControls.Count);
                Children.Add(TrackItemControls[i]);
            }

            Children.Add(TrackItemWaveVizualizerArea);
        }

        /// <summary>
        /// This method adds a slider control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="sliderLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric data for slider control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the slider is changed.</param>
        public void addSliderControl(int controlGroupID, string sliderLabel, Global.enumBaseColor color, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            TrackItemControls[controlGroupID].addSliderControl(sliderLabel, color, numericSliderData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds a picker control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="pickerLabel">The label string for the picker control.</param>
        /// <param name="numericPickerData">The numeric data for picker control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addPickerControl(int controlGroupID, string pickerLabel, Global.enumBaseColor color, Global.structPickerData numericPickerData, EventHandler _valueChangedEvent)
        {
            TrackItemControls[controlGroupID].addPickerControl(pickerLabel, color, numericPickerData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds a switch button control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="switchLabel">The label string for the picker control.</param>
        /// <param name="numericSwitchData">The numeric data for picker control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addSwitchControl(int controlGroupID, string switchLabel, Global.enumBaseColor color, Global.structSwitchData numericSwitchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            TrackItemControls[controlGroupID].addSwitchControl(switchLabel, color, numericSwitchData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds control group to the DSP UI Component
        /// </summary>
        public void addControlGroup()
        {
            TrackItemSliderControlGroup newCG = new TrackItemSliderControlGroup(this);

            TrackItemControls.Add(newCG);
            Children.Add(newCG);
            this.SetOrder(newCG, Children.Count - 1);
            this.SetOrder(TrackItemWaveVizualizerArea, Children.Count);

            for (int i = 0; i < TrackItemControls.Count; i++)
            {
                this.SetGrow(TrackItemControls[i], 0.55f / TrackItemControls.Count);
            }
        }

        //==============================================Events==============================================

        /// <summary>
        /// This method deletes a DSP event from the UI and from the audio systems
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            (Parent as VerticalStackLayout).Children.Remove(this);

            Global.structVariableDataTypeUnit dataTypeUnit = new Global.structVariableDataTypeUnit();
            dataTypeUnit.dataUnit = this;
            
            switch (dSPType)
            {
                case Global.enumDSPType.DISTORTION:
                    dataTypeUnit.dataType = Global.enumVariableDataType.TYPE_DISTORTION_DSP_CLASS;
                    break;
                case Global.enumDSPType.COMPRESSOR:
                    dataTypeUnit.dataType = Global.enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS;
                    break;
                case Global.enumDSPType.REVERB:
                    dataTypeUnit.dataType = Global.enumVariableDataType.TYPE_REVERB_DSP_CLASS;
                    break;
                case Global.enumDSPType.CHORUS:
                    dataTypeUnit.dataType = Global.enumVariableDataType.TYPE_CHORUS_DSP_CLASS;
                    break;
            }

            trackAudioProvider.removeDSPEffect(dataTypeUnit);
        }

        /// <summary>
        /// This method stops the wave visualization graph update worker
        /// </summary>
        public void stopGraphUpdateWorker()
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
        }
    }
}