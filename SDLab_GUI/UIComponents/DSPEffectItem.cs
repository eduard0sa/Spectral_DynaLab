using SDLab_GUI.AudioSystemsLogic;

namespace SDLab_GUI.UIComponents
{
    /// <summary>
    /// DSP effect UI component class
    /// <para>It connects the UI controls and sound systems in order to enable DSP control by the user.</para>
    /// </summary>
    /// <typeparam name="T">The DSPEffect SFX Type class (eg:.DistortionDSP, CompressorDSP)</typeparam>
    public class DSPEffectItem<T> : FlexLayout
    {
        private JuceAudioProvider oscAudioProvider;
        private MainPage mainPageOBJ;
        T dspProcessor;
        Global.enumDSPType dSPType;

        private DSPEffectItemTriangleMark DSPEffectTriangleMark;
        private DSPEffectItemHeader DSPEffectHeader;
        private List<DSPEffectItemControlGroup> DSPEffectSliderControls;
        private DSPEffectItemWaveVizualizerArea DSPEffectWaveVizualizerArea;

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
            DSPEffectTriangleMark = new DSPEffectItemTriangleMark(this);
            DSPEffectTriangleMark.ClickEventHandler = deleteDSPEvent;
            DSPEffectHeader = new DSPEffectItemHeader(this, _dspType);
            DSPEffectSliderControls = new List<DSPEffectItemControlGroup>();
            DSPEffectSliderControls.Add(new DSPEffectItemControlGroup(this));
            DSPEffectWaveVizualizerArea = new DSPEffectItemWaveVizualizerArea(this, _graphUpdateMehod, audioProvider);

            oscAudioProvider = audioProvider;
            dspProcessor = _dspProcessor;
            dSPType = _dspType;

            UIPaint();
        }

        /// <summary>
        /// UI sub-components painting method
        /// </summary>
        private void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);


            Children.Add(DSPEffectTriangleMark);
            Children.Add(DSPEffectHeader);

            for(int i = 0; i < DSPEffectSliderControls.Count; i++)
            {
                this.SetGrow(DSPEffectSliderControls[i], 0.55f / DSPEffectSliderControls.Count);
                Children.Add(DSPEffectSliderControls[i]);
            }

            Children.Add(DSPEffectWaveVizualizerArea);
        }

        /// <summary>
        /// This method adds a slider control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="sliderLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric data for slider control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the slider is changed.</param>
        public void addSliderControl(int controlGroupID, string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds a picker control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="pickerLabel">The label string for the picker control.</param>
        /// <param name="numericPickerData">The numeric data for picker control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addPickerControl(int controlGroupID, string pickerLabel, Global.structPickerData numericPickerData, EventHandler _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addPickerControl(pickerLabel, numericPickerData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds a switch button control to a control group in the DSP effect UI, enabling the user the change a property value.
        /// </summary>
        /// <param name="controlGroupID">The target control group box.</param>
        /// <param name="switchLabel">The label string for the picker control.</param>
        /// <param name="numericSwitchData">The numeric data for picker control definition.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addSwitchControl(int controlGroupID, string switchLabel, Global.structSliderData numericSwitchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addSwitchControl(switchLabel, numericSwitchData, _valueChangedEvent);
        }

        /// <summary>
        /// This method adds control group to the DSP UI Component
        /// </summary>
        public void addControlGroup()
        {
            DSPEffectItemControlGroup newCG = new DSPEffectItemControlGroup(this);

            DSPEffectSliderControls.Add(newCG);
            Children.Add(newCG);
            this.SetOrder(newCG, Children.Count - 1);
            this.SetOrder(DSPEffectWaveVizualizerArea, Children.Count);

            for (int i = 0; i < DSPEffectSliderControls.Count; i++)
            {
                this.SetGrow(DSPEffectSliderControls[i], 0.55f / DSPEffectSliderControls.Count);
            }
        }

        //==============================================Events==============================================

        /// <summary>
        /// This method deletes a DSP event from the UI and from the audio systems
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteDSPEvent(object? sender, EventArgs e)
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

            oscAudioProvider.removeDSPEffect(dataTypeUnit);
        }

        /// <summary>
        /// This method stops the wave visualization graph update worker
        /// </summary>
        public void stopGraphUpdateWorker()
        {
            DSPEffectWaveVizualizerArea.UpdateFrameTimer.Stop();
        }
    }

    #region Headers

    /// <summary>
    /// DSPEffect UI Component status block.
    /// <para>Allows the user to enable, disable and delete a DSP processor.</para>
    /// </summary>
    public class DSPEffectItemTriangleMark : FlexLayout
    {
        private ImageButton triangleMarkBTN = new ImageButton();
        private ImageButton deleteOscillatorBTN = new ImageButton();
        private EventHandler clickEventHandler;

        /// <summary>
        /// Property that stores the function that's executed by DSPEffect UI Component's delete button when it is clicked.
        /// </summary>
        public EventHandler ClickEventHandler
        {
            get
            {
                return clickEventHandler;
            }
            set
            {
                deleteOscillatorBTN.Clicked -= ClickEventHandler;
                clickEventHandler = value;
                deleteOscillatorBTN.Clicked += value;
            }
        }

        /// <summary>
        /// DSPEffectItemTriangleMark class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent DSP UI Component.</param>
        public DSPEffectItemTriangleMark(FlexLayout parentFLNode)
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            parentFLNode.SetGrow(this, 0.05f);

            triangleMarkBTN.Source = "triangle_mark.png";
            triangleMarkBTN.WidthRequest = 5;
            triangleMarkBTN.HeightRequest = 5;
            triangleMarkBTN.Padding = 10;
            triangleMarkBTN.BackgroundColor = Color.FromArgb("#14141d");

            deleteOscillatorBTN.Source = "trash_bin.png";
            deleteOscillatorBTN.WidthRequest = 5;
            deleteOscillatorBTN.HeightRequest = 5;
            deleteOscillatorBTN.Padding = 10;
            deleteOscillatorBTN.BackgroundColor = Color.FromArgb("#14141d");

            Children.Add(triangleMarkBTN);
            Children.Add(deleteOscillatorBTN);
        }
    }

    /// <summary>
    /// DSPEffect UI Component Header Block.
    /// </summary>
    public class DSPEffectItemHeader : FlexLayout
    {
        Label HeaderLabel = new Label();

        /// <summary>
        /// DSPEffectItemHeader class constructor
        /// </summary>
        /// <param name="parentFLNode">Reference to parent DSPEffect UI Component.</param>
        /// <param name="dspType">The kind of DSPProcessor effect (eg:. DISTORTION, COMPRESSOR, etc...)</param>
        public DSPEffectItemHeader(FlexLayout parentFLNode, Global.enumDSPType dspType)
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            parentFLNode.SetGrow(this, 0.20f);

            HeaderLabel.StyleClass = new List<string> { "GreenTitle" };
            HeaderLabel.Text = dspType.ToString();

            switch (dspType)
            {
                case Global.enumDSPType.DISTORTION:
                    HeaderLabel.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];
                    break;
                case Global.enumDSPType.COMPRESSOR:
                    HeaderLabel.TextColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
                    break;
                case Global.enumDSPType.REVERB:
                    HeaderLabel.TextColor = (Color)Application.Current.Resources["DefaultManaGreen"];
                    break;
                case Global.enumDSPType.EQ:
                    HeaderLabel.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];
                    break;
                case Global.enumDSPType.FILTER:
                    HeaderLabel.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];
                    break;
            }

            Children.Add(HeaderLabel);
        }
    }

    #endregion

    #region Slider Controls

    /// <summary>
    /// DSPEffect UI Component Control Group.
    /// </summary>
    public class DSPEffectItemControlGroup : FlexLayout
    {
        List<Global.structVariableDataTypeUnit> controlList = new List<Global.structVariableDataTypeUnit>();

        /// <summary>
        /// DSPEffectItemControlGroup class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to parent DSPEffect UI Component.</param>
        public DSPEffectItemControlGroup(FlexLayout parentFLNode)
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            VerticalOptions = LayoutOptions.Fill;
            
        }

        /// <summary>
        /// This method adds a slider control to this control group, allowing the user to change SFX property values.
        /// </summary>
        /// <param name="sliderLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric data group for the slider control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the slider is changed.</param>
        public void addSliderControl(string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            DSPEffectItemSliderControl sliderControl = new DSPEffectItemSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);

            Global.structVariableDataTypeUnit sliderControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = sliderControl,
                dataType = Global.enumVariableDataType.TYPE_SLIDER
            };

            controlList.Add(sliderControlDataUnit);

            //Update the vertical size of the other controls in the control group
            updateControlsFlexGrow();

            Children.Add(sliderControl);
        }

        /// <summary>
        /// This method adds a picker control to this control group, allowing the user to change SFX property values.
        /// </summary>
        /// <param name="pickerLabel">The label string for the picker control.</param>
        /// <param name="pickerData">The numeric data group for the picker control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addPickerControl(string pickerLabel, Global.structPickerData pickerData, EventHandler _valueChangedEvent)
        {
            DSPEffectItemPickerControl pickerControl = new DSPEffectItemPickerControl(pickerLabel, pickerData, _valueChangedEvent);

            Global.structVariableDataTypeUnit pickerControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = pickerControl,
                dataType = Global.enumVariableDataType.TYPE_PICKER
            };

            controlList.Add(pickerControlDataUnit);

            //Update the vertical size of the other controls in the control group
            updateControlsFlexGrow();

            Children.Add(pickerControl);
        }

        /// <summary>
        /// This method adds a picker control to this control group, allowing the user to change SFX property values.
        /// </summary>
        /// <param name="switchLabel">The label string for the picker control.</param>
        /// <param name="switchData">The numeric data group for the picker control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public void addSwitchControl(string switchLabel, Global.structSliderData switchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            DSPEffectItemSwitchControl switchControl = new DSPEffectItemSwitchControl(switchLabel, switchData, _valueChangedEvent);

            Global.structVariableDataTypeUnit switchControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = switchControl,
                dataType = Global.enumVariableDataType.TYPE_SWITCH
            };

            controlList.Add(switchControlDataUnit);

            //Update the vertical size of the other controls in the control group
            updateControlsFlexGrow();

            Children.Add(switchControl);
        }

        /// <summary>
        /// This method updates the dynamic size (relative to the parent, which is a FlexLayout object) of the controls attached to this control group, in order to adapt the number of controls to the control group height.
        /// </summary>
        private void updateControlsFlexGrow()
        {
            for (int i = 0; i < controlList.Count; i++)
            {
                switch (controlList[i].dataType)
                {
                    case Global.enumVariableDataType.TYPE_SLIDER:
                        this.SetGrow(controlList[i].dataUnit as DSPEffectItemSliderControl, 1.0f / controlList.Count);
                        break;
                    case Global.enumVariableDataType.TYPE_PICKER:
                        this.SetGrow(controlList[i].dataUnit as DSPEffectItemPickerControl, 1.0f / controlList.Count);
                        break;
                    case Global.enumVariableDataType.TYPE_SWITCH:
                        this.SetGrow(controlList[i].dataUnit as DSPEffectItemSwitchControl, 1.0f / controlList.Count);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// DSPEffect UI Component Slider Control Class.
    /// </summary>
    public class DSPEffectItemSliderControl : HorizontalStackLayout
    {
        Label sliderControlLabel = new Label();
        Slider sliderControlSlider = new Slider();
        Entry sliderControlEntry = new Entry();
        Global.structSliderData _numericSliderData_;

        bool isUpdatingEntry = false;

        /// <summary>
        /// DSPEffectItemSliderControl class constructor.
        /// </summary>
        /// <param name="controlLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric data block for the slider control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the slider is changed.</param>
        public DSPEffectItemSliderControl(string controlLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            _numericSliderData_ = numericSliderData;

            Padding = new Thickness(20, 0, 0, 0);

            sliderControlLabel.Text = controlLabel;
            sliderControlLabel.VerticalOptions = LayoutOptions.Center;

            sliderControlSlider.HorizontalOptions = LayoutOptions.Fill;
            sliderControlSlider.VerticalOptions = LayoutOptions.Center;
            sliderControlSlider.Focused += SliderAutoUnfocusEvent;
            sliderControlSlider.StyleClass = new List<string> { "RedAudioAttributeSlider" };

            sliderControlSlider.Minimum = numericSliderData.minVal;
            sliderControlSlider.Maximum = numericSliderData.maxVal;
            sliderControlSlider.Value = numericSliderData.defVal;
            sliderControlSlider.ValueChanged += _valueChangedEvent;
            sliderControlSlider.DragCompleted += sliderDragCompletedEvent;

            sliderControlEntry.WidthRequest = 100;
            sliderControlEntry.HeightRequest = 1;
            sliderControlEntry.VerticalOptions = LayoutOptions.Center;
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{numericSliderData.numDisplayDecPlaces}");
            sliderControlEntry.BackgroundColor = Color.FromArgb("#14141d");
            sliderControlEntry.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            sliderControlEntry.TextChanged += entryValueChangedEvent;

            Children.Add(sliderControlLabel);
            Children.Add(sliderControlSlider);
            Children.Add(sliderControlEntry);
        }

        /// <summary>
        /// This method automatically unfocus the slider when it is automatically focused.
        /// </summary>
        /// <param name="sender">The slider sender object reference.</param>
        /// <param name="e">The Event Handler's Focus Event Arguments.</param>
        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        /// <summary>
        /// This method updates the value of the control text entry when the user finishes changing the value of the slider (on thumb release).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderDragCompletedEvent(object? sender, EventArgs e)
        {
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{_numericSliderData_.numDisplayDecPlaces}");
            isUpdatingEntry = true;
        }

        /// <summary>
        /// This method updates the slider value when control's text entry value is manually modified by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryValueChangedEvent(object? sender, TextChangedEventArgs e)
        {
            if (float.TryParse(e.NewTextValue, out float newValue) && isUpdatingEntry == false)
            {
                if (newValue >= _numericSliderData_.minVal && newValue <= _numericSliderData_.maxVal)
                {
                    sliderControlSlider.Value = newValue;
                }
            }
            else
            {
                isUpdatingEntry = false;
            }
        }
    }

    /// <summary>
    /// DSPEffect UI Component Picker Control.
    /// </summary>
    public class DSPEffectItemPickerControl : HorizontalStackLayout
    {
        Label pickerControlLabel = new Label();
        Picker pickerControlPicker = new Picker();
        Global.structPickerData _numericSliderData_;

        /// <summary>
        /// DSPEffectItemPickerControl class constructor.
        /// </summary>
        /// <param name="controlLabel">The label string for this picker control.</param>
        /// <param name="numericPickerData">The numeric data for picker control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the picker is changed.</param>
        public DSPEffectItemPickerControl(string controlLabel, Global.structPickerData numericPickerData, EventHandler _valueChangedEvent)
        {
            _numericSliderData_ = numericPickerData;

            Padding = new Thickness(20, 0, 0, 0);

            pickerControlLabel.Text = controlLabel;
            pickerControlLabel.VerticalOptions = LayoutOptions.Center;

            pickerControlPicker.WidthRequest = 250;
            pickerControlPicker.VerticalOptions = LayoutOptions.Center;
            pickerControlPicker.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];

            foreach (string item in _numericSliderData_.items)
            {
                pickerControlPicker.Items.Add(item);
            }

            pickerControlPicker.SelectedIndex = numericPickerData.defValIndex;
            pickerControlPicker.SelectedIndexChanged += _valueChangedEvent;

            Children.Add(pickerControlLabel);
            Children.Add(pickerControlPicker);
        }
    }

    /// <summary>
    /// DSPEffect UI Component Switch Control.
    /// </summary>
    public class DSPEffectItemSwitchControl : HorizontalStackLayout
    {
        Label switchControlLabel = new Label();
        Switch switchControlSwitch = new Switch();
        Global.structSliderData _numericSwitchData_;

        /// <summary>
        /// DSPEffectItemSwitchControl class constructor.
        /// </summary>
        /// <param name="controlLabel">The label string for this switch control.</param>
        /// <param name="numericSwitchData">The numeric data for the switch control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed by the event handler when the value of the switch is changed.</param>
        public DSPEffectItemSwitchControl(string controlLabel, Global.structSliderData numericSwitchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            _numericSwitchData_ = numericSwitchData;

            Padding = new Thickness(20, 0, 0, 0);

            switchControlLabel.Text = controlLabel;
            switchControlLabel.VerticalOptions = LayoutOptions.Center;

            switchControlSwitch.WidthRequest = 50;
            switchControlSwitch.VerticalOptions = LayoutOptions.Center;
            switchControlSwitch.OnColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            switchControlSwitch.ThumbColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            switchControlSwitch.Margin = new Thickness(20, 0, 0, 0);

            switchControlSwitch.IsToggled = false;
            switchControlSwitch.Toggled += _valueChangedEvent;

            Children.Add(switchControlLabel);
            Children.Add(switchControlSwitch);
        }
    }

    #endregion

    #region Wave Shape Controls

    /// <summary>
    /// DSPEffect UI Component Wave Visualizer Area.
    /// </summary>
    public class DSPEffectItemWaveVizualizerArea : StackLayout
    {
        DSPEffectItemWaveVizualizerGV visualizer;
        Microsoft.Maui.Dispatching.IDispatcherTimer updateFrameTimer;

        /// <summary>
        /// DSPEffectItemWaveVizualizerArea class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout DSPEffect UI Component.</param>
        /// <param name="_graphUpdateFunction">Provides the function executed whe the graph visualization is updated.</param>
        /// <param name="osc">Reference to DSPEffect parent oscillator UI object.</param>
        public DSPEffectItemWaveVizualizerArea(FlexLayout parentFLNode, Func<float[]> _graphUpdateFunction, JuceAudioProvider osc)
        {
            parentFLNode.SetGrow(this, 0.20f);
            HeightRequest = 100;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = (Color)Application.Current.Resources["Gray950"];

            visualizer = new DSPEffectItemWaveVizualizerGV(this, osc);

            Children.Add(visualizer);

            UpdateFrameTimer = Dispatcher.CreateTimer();

            UpdateFrameTimer.Interval = TimeSpan.FromMilliseconds(33);

            UpdateFrameTimer.Tick += delegate {
                visualizer.VisSamplesArray = _graphUpdateFunction();
                visualizer.updateWaveForm();
            };

            UpdateFrameTimer.Start();
        }

        /// <summary>
        /// Timer routine that runs the graph updater method each 0.033s (30 FPS).
        /// </summary>
        public IDispatcherTimer UpdateFrameTimer { get => updateFrameTimer; set => updateFrameTimer = value; }
    }

    /// <summary>
    /// DSPEffect UI Component wave visualizer Graphics View
    /// </summary>
    public class DSPEffectItemWaveVizualizerGV : GraphicsView
    {
        SoundWaveShapeDrawable drawableEngine;
        JuceAudioProvider AP;

        /// <summary>
        /// The floating-point sample array that contains the sound wave data for displaying purposes.
        /// </summary>
        public float[] VisSamplesArray
        {
            get
            {
                return drawableEngine.VisSamplesArray;
            }
            set
            {
                drawableEngine.VisSamplesArray = value;
            }
        }

        /// <summary>
        /// DSPEffectItemWaveVizualizerGV class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout DSPEffect UI Component.</param>
        /// <param name="osc">Reference to DSPEffect parent oscillator UI object.</param>
        public DSPEffectItemWaveVizualizerGV(StackLayout parentFLNode, JuceAudioProvider osc)
        {
            HorizontalOptions = LayoutOptions.Fill;
            HeightRequest = 60;
            drawableEngine = new SoundWaveShapeDrawable();
            Drawable = drawableEngine;
            AP = osc;
            updateWaveForm();
        }

        /// <summary>
        /// This method updates wave visualizer's graph.
        /// </summary>
        public void updateWaveForm()
        {
            drawableEngine.OscAP = AP;
            Invalidate();
        }
    }

    #endregion
}