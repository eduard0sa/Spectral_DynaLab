using SDLab_GUI.AudioSystemsLogic;
using System.Windows.Forms.VisualStyles;

namespace SDLab_GUI.UIComponents
{
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

        public T DspProcessor { get => dspProcessor; }

        public DSPEffectItem(JuceAudioProvider audioProvider, Global.enumDSPType _dspType, T _dspProcessor, Func<float[]> _graphUpdateMehod)
        {
            DSPEffectTriangleMark = new DSPEffectItemTriangleMark(this);
            DSPEffectTriangleMark.ClickEventHandler = deleteDSPEvent;
            DSPEffectHeader = new DSPEffectItemHeader(this, _dspType);
            DSPEffectSliderControls = new List<DSPEffectItemControlGroup>();
            DSPEffectSliderControls.Add(new DSPEffectItemControlGroup(this));
            DSPEffectWaveVizualizerArea = new DSPEffectItemWaveVizualizerArea(this, _graphUpdateMehod);

            oscAudioProvider = audioProvider;
            dspProcessor = _dspProcessor;
            dSPType = _dspType;

            UIPaint();
        }

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

        public void addSliderControl(int controlGroupID, string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);
        }

        public void addPickerControl(int controlGroupID, string pickerLabel, Global.structPickerData numericPickerData, EventHandler _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addPickerControl(pickerLabel, numericPickerData, _valueChangedEvent);
        }

        public void addSwitchControl(int controlGroupID, string switchLabel, Global.structSliderData numericSwitchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            DSPEffectSliderControls[controlGroupID].addSwitchControl(switchLabel, numericSwitchData, _valueChangedEvent);
        }

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

        //Events

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
    }

    #region Headers

    public class DSPEffectItemTriangleMark : FlexLayout
    {
        private ImageButton triangleMarkBTN = new ImageButton();
        private ImageButton deleteOscillatorBTN = new ImageButton();
        private EventHandler clickEventHandler;

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

    public class DSPEffectItemHeader : FlexLayout
    {
        Label HeaderLabel = new Label();
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
    public class DSPEffectItemControlGroup : FlexLayout
    {
        List<Global.structVariableDataTypeUnit> controlList = new List<Global.structVariableDataTypeUnit>();
        public DSPEffectItemControlGroup(FlexLayout parentFLNode)
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            VerticalOptions = LayoutOptions.Fill;
            
        }

        public void addSliderControl(string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            DSPEffectItemSliderControl sliderControl = new DSPEffectItemSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);

            Global.structVariableDataTypeUnit sliderControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = sliderControl,
                dataType = Global.enumVariableDataType.TYPE_SLIDER
            };

            controlList.Add(sliderControlDataUnit);

            updateControlsFlexGrow();

            Children.Add(sliderControl);
        }

        public void addPickerControl(string sliderLabel, Global.structPickerData pickerData, EventHandler _valueChangedEvent)
        {
            DSPEffectItemPickerControl pickerControl = new DSPEffectItemPickerControl(sliderLabel, pickerData, _valueChangedEvent);

            Global.structVariableDataTypeUnit pickerControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = pickerControl,
                dataType = Global.enumVariableDataType.TYPE_PICKER
            };

            controlList.Add(pickerControlDataUnit);

            updateControlsFlexGrow();

            Children.Add(pickerControl);
        }

        public void addSwitchControl(string switchLabel, Global.structSliderData switchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
        {
            DSPEffectItemSwitchControl switchControl = new DSPEffectItemSwitchControl(switchLabel, switchData, _valueChangedEvent);

            Global.structVariableDataTypeUnit switchControlDataUnit = new Global.structVariableDataTypeUnit()
            {
                dataUnit = switchControl,
                dataType = Global.enumVariableDataType.TYPE_SWITCH
            };

            controlList.Add(switchControlDataUnit);

            updateControlsFlexGrow();

            Children.Add(switchControl);
        }

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

    public class DSPEffectItemSliderControl : HorizontalStackLayout
    {
        Label sliderControlLabel = new Label();
        Slider sliderControlSlider = new Slider();
        Entry sliderControlEntry = new Entry();
        Global.structSliderData _numericSliderData_;

        bool isUpdatingEntry = false;

        public DSPEffectItemSliderControl(string controlLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            _numericSliderData_ = numericSliderData;

            Padding = new Thickness(20, 0, 0, 0);

            sliderControlLabel.Text = controlLabel;
            sliderControlLabel.VerticalOptions = LayoutOptions.Center;

            //sliderControlSlider.WidthRequest = 300;
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

        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        private void sliderDragCompletedEvent(object? sender, EventArgs e)
        {
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{_numericSliderData_.numDisplayDecPlaces}");
            isUpdatingEntry = true;
        }

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

    public class DSPEffectItemPickerControl : HorizontalStackLayout
    {
        Label pickerControlLabel = new Label();
        Picker pickerControlPicker = new Picker();
        Global.structPickerData _numericSliderData_;

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

    public class DSPEffectItemSwitchControl : HorizontalStackLayout
    {
        Label switchControlLabel = new Label();
        Switch switchControlSwitch = new Switch();
        Global.structSliderData _numericSwitchData_;

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

    public class DSPEffectItemWaveVizualizerArea : StackLayout
    {
        DSPEffectItemWaveVizualizerGV visualizer;
        public DSPEffectItemWaveVizualizerArea(FlexLayout parentFLNode, Func<float[]> _graphUpdateFunction)
        {
            parentFLNode.SetGrow(this, 0.20f);
            HeightRequest = 100;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = (Color)Application.Current.Resources["Gray950"];

            visualizer = new DSPEffectItemWaveVizualizerGV(this);

            Children.Add(visualizer);

            Dispatcher.StartTimer(
                TimeSpan.FromMilliseconds(33), // ~60 FPS
                () =>
                {
                    /*PullWaveformFromDll();*/
                    visualizer.VisSamplesArray = _graphUpdateFunction();
                    visualizer.updateWaveForm();
                    return true;
                }
            );
        }
    }

    public class DSPEffectItemWaveVizualizerGV : GraphicsView
    {
        SoundWaveShapeDrawable drawableEngine;

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

        public DSPEffectItemWaveVizualizerGV(StackLayout parentFLNode)
        {
            HorizontalOptions = LayoutOptions.Fill;
            HeightRequest = 60;
            drawableEngine = new SoundWaveShapeDrawable();
            Drawable = drawableEngine;
            updateWaveForm();
        }

        public void updateWaveForm()
        {
            Invalidate();
        }
    }

    #endregion
}