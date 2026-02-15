using SDLab_GUI.AudioSystemsLogic;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    /// <summary>
    /// Template UI audio track component class (abstract class). Can be used as base class for oscillators, audio file players, etc....
    /// </summary>
    internal abstract class TrackItem : FlexLayout
    {
        protected JuceAudioProvider trackAudioProvider;
        protected AudioEngineMGMT audioEngineMGMT;
        protected MainPage mainPageOBJ;

        protected TrackItemLeftIconMenu TrackTriangleMark;
        protected TrackItemHeader TrackItemHeader;
        protected List<TrackItemSliderControlGroup> TrackItemControls;
        protected TrackItemWaveVizualizerArea TrackItemWaveVizualizerArea;

        protected structSliderData gainSliderData;

        protected abstract void UIPaint();
        protected abstract void deleteTrackEvent(object? sender, EventArgs e);
    }

    /// <summary>
    /// Track Item UI Component status block.
    /// </summary>
    public class TrackItemLeftIconMenu : FlexLayout
    {
        private ImageButton triangleMarkBTN = new ImageButton();
        private ImageButton deleteTrackBTN = new ImageButton();
        private EventHandler clickEventHandler;

        /// <summary>
        /// This property holds the function that is executed when the user clicks on delete track button.
        /// </summary>
        public EventHandler ClickEventHandler
        {
            get
            {
                return clickEventHandler;
            }
            set
            {
                deleteTrackBTN.Clicked -= ClickEventHandler;
                clickEventHandler = value;
                deleteTrackBTN.Clicked += value;
            }
        }

        /// <summary>
        /// TrackItemLeftIconMenu class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the status block FlexLayout Parent (the track UI Component).</param>
        public TrackItemLeftIconMenu(FlexLayout parentFLNode)
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

            deleteTrackBTN.Source = "trash_bin.png";
            deleteTrackBTN.WidthRequest = 5;
            deleteTrackBTN.HeightRequest = 5;
            deleteTrackBTN.Padding = 10;
            deleteTrackBTN.BackgroundColor = Color.FromArgb("#14141d");

            Children.Add(triangleMarkBTN);
            Children.Add(deleteTrackBTN);
        }
    }

    /// <summary>
    /// Track Item UI Component Header
    /// </summary>
    internal class TrackItemHeader : FlexLayout
    {
        Label HeaderLabel = new Label();

        /// <summary>
        /// TrackItemHeader class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the item header FlexLayout Parent (the track UI Component).</param>
        public TrackItemHeader(FlexLayout parentFLNode, string displayText, List<string> styleClasses)
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            parentFLNode.SetGrow(this, 0.10f);

            HeaderLabel.Text = displayText;
            HeaderLabel.StyleClass = styleClasses;

            Children.Add(HeaderLabel);
        }
    }

    /// <summary>
    /// Track Item UI Component Slider Control Group.
    /// </summary>
    internal class TrackItemSliderControlGroup : FlexLayout
    {
        List<Global.structVariableDataTypeUnit> controlList = new List<Global.structVariableDataTypeUnit>();

        /// <summary>
        /// TrackItemSliderControlGroup class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the item slider control group FlexLayout Parent (the track UI Component).</param>
        public TrackItemSliderControlGroup(FlexLayout parentFLNode)
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;
            parentFLNode.SetGrow(this, 0.26f);
        }

        /// <summary>
        /// This method adds a slider control to the slider control group, allowing the user to manipulate audio properties using a slider.
        /// </summary>
        /// <param name="sliderLabel">The Label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric data for slider control's setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed when the slider value is changed by the user.</param>
        public void addSliderControl(string sliderLabel, enumBaseColor color, structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            TrackItemSliderControl sliderControl = new TrackItemSliderControl(sliderLabel, color, numericSliderData, _valueChangedEvent);

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
            TrackItemPickerControl pickerControl = new TrackItemPickerControl(pickerLabel, pickerData, _valueChangedEvent);

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
            TrackItemSwitchControl switchControl = new TrackItemSwitchControl(switchLabel, switchData, _valueChangedEvent);

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
        /// This method updates all slider control's dynamic height (on a the SliderControlGroup), allowing responsive resizing when a slider is added or removed from this control group.
        /// </summary>
        private void updateControlsFlexGrow()
        {
            for (int i = 0; i < controlList.Count; i++)
            {
                switch (controlList[i].dataType)
                {
                    case Global.enumVariableDataType.TYPE_SLIDER:
                        this.SetGrow(controlList[i].dataUnit as TrackItemSliderControl, 1.0f / controlList.Count);
                        break;
                    case Global.enumVariableDataType.TYPE_PICKER:
                        this.SetGrow(controlList[i].dataUnit as TrackItemPickerControl, 1.0f / controlList.Count);
                        break;
                    case Global.enumVariableDataType.TYPE_SWITCH:
                        this.SetGrow(controlList[i].dataUnit as TrackItemSwitchControl, 1.0f / controlList.Count);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Track Item UI Component Slider Control.
    /// </summary>
    internal class TrackItemSliderControl : FlexLayout
    {
        Label sliderControlLabel = new Label();
        Slider sliderControlSlider = new Slider();
        Entry sliderControlEntry = new Entry();
        structSliderData _numericSliderData_;

        bool isUpdatingEntry = false;

        /// <summary>
        /// TrackItemSliderControl class constructor.
        /// </summary>
        /// <param name="controlLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric date for the slider control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed when the slider value is changed by the user.</param>
        public TrackItemSliderControl(string controlLabel, enumBaseColor baseColor, structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            _numericSliderData_ = numericSliderData;

            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            Padding = new Thickness(20, 0, 0, 0);

            sliderControlLabel.Text = controlLabel;

            sliderControlSlider.WidthRequest = 200;
            sliderControlSlider.Focused += SliderAutoUnfocusEvent;

            switch (baseColor)
            {
                case enumBaseColor.YELLOW:
                    sliderControlSlider.StyleClass = new List<string> { "AudioAttributeSlider" };
                    sliderControlEntry.TextColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
                    break;
                case enumBaseColor.RED:
                    sliderControlSlider.StyleClass = new List<string> { "RedAudioAttributeSlider" };
                    sliderControlEntry.TextColor = (Color)Application.Current.Resources["DefaultPastelRed"];
                    break;
                case enumBaseColor.GREEN:
                    sliderControlSlider.StyleClass = new List<string> { "GreenAudioAttributeSlider" };
                    sliderControlEntry.TextColor = (Color)Application.Current.Resources["GreenAudioAttributeSlider"];
                    break;
            }

            sliderControlSlider.Minimum = numericSliderData.minVal;
            sliderControlSlider.Maximum = numericSliderData.maxVal;
            sliderControlSlider.Value = numericSliderData.defVal;
            sliderControlSlider.ValueChanged += _valueChangedEvent;
            sliderControlSlider.DragCompleted += sliderDragCompletedEvent;

            sliderControlEntry.WidthRequest = 100;
            sliderControlEntry.HeightRequest = 1;
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{numericSliderData.numDisplayDecPlaces}");
            sliderControlEntry.BackgroundColor = Color.FromArgb("#14141d");
            sliderControlEntry.TextChanged += entryValueChangedEvent;

            Children.Add(sliderControlLabel);
            Children.Add(sliderControlSlider);
            Children.Add(sliderControlEntry);
        }

        /// <summary>
        /// This method automatically unfocus the slider control when it is automatically focused.
        /// </summary>
        /// <param name="sender">The slider sender object reference.</param>
        /// <param name="e">Event Handler's Event Arguments data.</param>
        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        /// <summary>
        /// This method updates the value of the control text entry when the user finishes changing the slider value (on thumb released).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderDragCompletedEvent(object? sender, EventArgs e)
        {
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{_numericSliderData_.numDisplayDecPlaces}");
            isUpdatingEntry = true;
        }

        /// <summary>
        /// This method changes the value of the slider control when the text entry is manually changed by the user.
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
    public class TrackItemPickerControl : HorizontalStackLayout
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
        public TrackItemPickerControl(string controlLabel, Global.structPickerData numericPickerData, EventHandler _valueChangedEvent)
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
    public class TrackItemSwitchControl : HorizontalStackLayout
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
        public TrackItemSwitchControl(string controlLabel, Global.structSliderData numericSwitchData, EventHandler<ToggledEventArgs> _valueChangedEvent)
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

    /// <summary>
    /// Track Item UI Component Wave Visualizer Area.
    /// </summary>
    internal class TrackItemWaveVizualizerArea : StackLayout
    {
        TrackItemWaveVizualizerGV visualizer;
        IDispatcherTimer updateFrameTimer;

        /// <summary>
        /// This property hold the function that updates the sound wave visualizer graph, every 0.033s (30 FPS).
        /// </summary>
        public IDispatcherTimer UpdateFrameTimer
        {
            get => updateFrameTimer;
            set => updateFrameTimer = value;
        }

        /// <summary>
        /// TrackItemWaveVizualizerArea class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Track Item UI Component).</param>
        /// <param name="_graphUpdateFunction">Provides the function that updates the sound wave visualizer graph.</param>
        /// <param name="_audioProvider">Reference to audio system's audio provider (oscillator, file track, etc...) engine object.</param>
        public TrackItemWaveVizualizerArea(FlexLayout parentFLNode, Func<float[]> _graphUpdateFunction, JuceAudioProvider _audioProvider)
        {
            parentFLNode.SetGrow(this, 0.16f);
            HeightRequest = 100;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = (Color)Application.Current.Resources["Gray950"];

            visualizer = new TrackItemWaveVizualizerGV(this, _audioProvider);

            Children.Add(visualizer);

            UpdateFrameTimer = Dispatcher.CreateTimer();

            UpdateFrameTimer.Interval = TimeSpan.FromMilliseconds(33);

            UpdateFrameTimer.Tick += delegate {
                visualizer.VisSamplesArray = _graphUpdateFunction();
                visualizer.updateWaveForm();
            };

            UpdateFrameTimer.Start();
        }
    }

    /// <summary>
    /// Track Item UI Component Wave Visualizer Graphics View.
    /// </summary>
    internal class TrackItemWaveVizualizerGV : GraphicsView
    {
        SoundWaveShapeDrawable drawableEngine = new SoundWaveShapeDrawable();
        JuceAudioProvider AP;

        /// <summary>
        /// This property holds the floating-point sample array for sound wave graph visualization.
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
        /// TrackItemWaveVizualizerGV constructor method.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Track Item UI Component).</param>
        /// <param name="_audioProvider">Reference to audio system's audio provider (oscillator, file track, etc...) engine object.</param>
        public TrackItemWaveVizualizerGV(StackLayout parentFLNode, JuceAudioProvider _audioProvider)
        {
            HorizontalOptions = LayoutOptions.Fill;
            HeightRequest = 60;
            Drawable = drawableEngine;
            BackgroundColor = (Color)Application.Current.Resources["Gray500"];
            AP = _audioProvider;
            updateWaveForm();
        }

        /// <summary>
        /// This method updates the sound wave visualizer graph.
        /// </summary>
        public void updateWaveForm()
        {
            drawableEngine.OscAP = AP;
            Invalidate();
        }
    }

    /// <summary>
    /// SoundWave Drawable class.
    /// </summary>
    public class SoundWaveShapeDrawable : IDrawable
    {
        float[] visSamplesArray;
        JuceAudioProvider oscAP;

        /// <summary>
        /// This property holds the floating-point array for sound wave graph visualization.
        /// </summary>
        public float[] VisSamplesArray
        {
            get
            {
                return visSamplesArray;
            }
            set
            {
                visSamplesArray = value;
            }
        }

        /// <summary>
        /// Reference to the parent Audio Provider object.
        /// </summary>
        public JuceAudioProvider OscAP
        {
            get => oscAP;
            set => oscAP = value;
        }

        /// <summary>
        /// This method updated the graph path, by reading the samples array and drawing it's values in the Graphics View.
        /// </summary>
        /// <param name="canvas">Target Graphics view object reference.</param>
        /// <param name="dirtyRect">Target Graphics view limit rectangle entity reference.</param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (VisSamplesArray != null)
            {
                int Samples = 450;
                float Amplitude = oscAP.CurrentGain;
                float Frequency = 1f; // in cycles over the width

                canvas.StrokeColor = (Color)Application.Current.Resources["DefaultPastelRed"];
                canvas.StrokeSize = 3;

                float midY = dirtyRect.Height / 2f;
                float width = dirtyRect.Width;
                float height = dirtyRect.Height;

                float stepX = width / (Samples - 1);

                PathF path = new PathF();

                for (int i = 0; i < Samples; i++)
                {
                    float x = i * stepX;

                    // Normalized x in range [0, 2π * Frequency]
                    float t = (float)i / (Samples - 1);

                    float y = midY - visSamplesArray[i] * Amplitude * 50; // scale to view

                    if (i == 0)
                        path.MoveTo(x, y);
                    else
                        path.LineTo(x, y);
                }

                canvas.DrawPath(path);
            }
        }
    }
}