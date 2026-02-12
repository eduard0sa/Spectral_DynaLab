using SDLab_GUI.AudioSystemsLogic;

namespace SDLab_GUI.UIComponents
{
    /// <summary>
    /// Oscillator Item UI Component.
    /// </summary>
    public class OscillatorItem : FlexLayout
    {
        private JuceAudioProvider oscAudioProvider;
        private AudioEngineMGMT audioEngineMGMT;
        private MainPage mainPageOBJ;

        private OscillatorItemTriangleMark OscillatorTriangleMark;
        private OscillatorItemHeader OscillatorItemHeader;
        private OscillatorItemSliderControlGroup OscillatorItemSliderControls;
        private OscillatorItemWaveShapeControl OscillatorItemWaveShapeControls;
        private OscillatorItemSFXButtonArea OscillatorItemSFXButtonArea;
        private OscillatorItemWaveVizualizerArea OscillatorItemWaveVizualizerArea;

        private Global.structSliderData frequencySliderData;
        private Global.structSliderData gainSliderData;

        /// <summary>
        /// OscillatorItem class constructor.
        /// </summary>
        /// <param name="audioManager">Reference to the audio manager instance from MainPage.</param>
        /// <param name="mainPage">Reference from the Main Page program instance.</param>
        public OscillatorItem(AudioEngineMGMT audioManager, MainPage mainPage)
        {
            OscillatorTriangleMark = new OscillatorItemTriangleMark(this);
            OscillatorTriangleMark.ClickEventHandler = deleteOscillatorEvent;
            OscillatorItemHeader = new OscillatorItemHeader(this);
            OscillatorItemSliderControls = new OscillatorItemSliderControlGroup(this);
            OscillatorItemWaveShapeControls = new OscillatorItemWaveShapeControl(this);
            OscillatorItemSFXButtonArea = new OscillatorItemSFXButtonArea(this);
            OscillatorItemSFXButtonArea.OpenSFXEditorEvent = openSFXEditorEvent;

            oscAudioProvider = audioManager.LaunchAudioEngine();
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;

            OscillatorItemWaveVizualizerArea = new OscillatorItemWaveVizualizerArea(this, oscAudioProvider.pushOSCVisSampleArray, oscAudioProvider);

            frequencySliderData = new Global.structSliderData() {
                minVal = 20f,
                maxVal = 2000f,
                defVal = oscAudioProvider.CurrentFrequency,
                numDisplayDecPlaces = 0
            };

            gainSliderData = new Global.structSliderData() {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = oscAudioProvider.CurrentGain,
                numDisplayDecPlaces = 2
            };

            UIPaint();
        }

        /// <summary>
        /// This method paints all the sub-components on the OscillatorItem UI Component.
        /// </summary>
        private void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);

            //Adding Slider Controls to the main Control Group.
            OscillatorItemSliderControls.addSliderControl("Frequency:", frequencySliderData, frequencyChangeEvent);
            OscillatorItemSliderControls.addSliderControl("Gain:", gainSliderData, gainChangeEvent);

            Children.Add(OscillatorTriangleMark);
            Children.Add(OscillatorItemHeader);
            Children.Add(OscillatorItemSliderControls);
            Children.Add(OscillatorItemWaveShapeControls);
            Children.Add(OscillatorItemSFXButtonArea);
            Children.Add(OscillatorItemWaveVizualizerArea);
        }

        //==============================================Events==============================================

        /// <summary>
        /// This method changes the frequency of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Frequency Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void frequencyChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newFrequency = (float)e.NewValue;
            oscAudioProvider.changeFrequency(newFrequency);
        }

        /// <summary>
        /// This method changes the gain of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Gain Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void gainChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newGain = (float)e.NewValue;
            oscAudioProvider.changeGain(newGain);
        }

        /// <summary>
        /// This method deletes the oscillator from the UI and from the audio systems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteOscillatorEvent(object? sender, EventArgs e)
        {
            OscillatorItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(oscAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);
        }

        /// <summary>
        /// This method opens the SFX Editor window when the user clicks on the proper UI button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSFXEditorEvent(object? sender, EventArgs e)
        {
            DSPModalEditor OscSFXDSPEditor = new DSPModalEditor(audioEngineMGMT, oscAudioProvider);
            mainPageOBJ.Navigation.PushModalAsync(OscSFXDSPEditor);
            OscSFXDSPEditor.ModalBoxCloseEvent += closeSFXEditorEvent;
        }

        /// <summary>
        /// This method closes the SFX Editor window when the user clicks on the proper UI button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeSFXEditorEvent(object? sender, EventArgs e)
        {
            mainPageOBJ.Navigation.PopModalAsync();
        }
    }

    #region Headers

    /// <summary>
    /// Oscillator Item UI Component status block.
    /// </summary>
    internal class OscillatorItemTriangleMark : FlexLayout
    {
        private ImageButton triangleMarkBTN = new ImageButton();
        private ImageButton deleteOscillatorBTN = new ImageButton();
        private EventHandler clickEventHandler;

        /// <summary>
        /// This property holds the function that is executed when the user clicks on delete oscillator button.
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
        /// OscillatorItemTriangleMark class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the status block FlexLayout Parent (the oscillator UI Component).</param>
        public OscillatorItemTriangleMark(FlexLayout parentFLNode)
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
    /// Oscillator Item UI Component Header
    /// </summary>
    internal class OscillatorItemHeader : FlexLayout
    {
        Label HeaderLabel = new Label();

        /// <summary>
        /// OscillatorItemHeader class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the item header FlexLayout Parent (the oscillator UI Component).</param>
        public OscillatorItemHeader(FlexLayout parentFLNode)
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            parentFLNode.SetGrow(this, 0.10f);

            HeaderLabel.Text = "OSCILLATOR";
            HeaderLabel.StyleClass = new List<string> { "GreenTitle" };

            Children.Add(HeaderLabel);
        }
    }

    #endregion

    #region Slider Controls

    /// <summary>
    /// Oscillator Item UI Component Slider Control Group.
    /// </summary>
    internal class OscillatorItemSliderControlGroup : FlexLayout
    {
        List<OscillatorItemSliderControl> sliderControlList = new List<OscillatorItemSliderControl>();

        /// <summary>
        /// OscillatorItemSliderControlGroup class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the item slider control group FlexLayout Parent (the oscillator UI Component).</param>
        public OscillatorItemSliderControlGroup(FlexLayout parentFLNode)
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
        public void addSliderControl(string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            OscillatorItemSliderControl sliderControl = new OscillatorItemSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);

            sliderControlList.Add(sliderControl);

            updateSliderControlsFlexGrow();

            Children.Add(sliderControl);
        }

        /// <summary>
        /// This method updates all slider control's dynamic height (on a the SliderControlGroup), allowing responsive resizing when a slider is added or removed from this control group.
        /// </summary>
        private void updateSliderControlsFlexGrow()
        {
            for(int i = 0; i < sliderControlList.Count; i++)
            {
                this.SetGrow(sliderControlList[i], 1.0f / sliderControlList.Count);
            }
        }
    }

    /// <summary>
    /// Oscillator Item UI Component Slider Control.
    /// </summary>
    internal class OscillatorItemSliderControl : FlexLayout
    {
        Label sliderControlLabel = new Label();
        Slider sliderControlSlider = new Slider();
        Entry sliderControlEntry = new Entry();
        Global.structSliderData _numericSliderData_;

        bool isUpdatingEntry = false;

        /// <summary>
        /// OscillatorItemSliderControl class constructor.
        /// </summary>
        /// <param name="controlLabel">The label string for the slider control.</param>
        /// <param name="numericSliderData">The numeric date for the slider control setup.</param>
        /// <param name="_valueChangedEvent">Provides the function that is executed when the slider value is changed by the user.</param>
        public OscillatorItemSliderControl(string controlLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            _numericSliderData_ = numericSliderData;

            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            Padding = new Thickness(20, 0, 0, 0);

            sliderControlLabel.Text = controlLabel;

            sliderControlSlider.WidthRequest = 200;
            sliderControlSlider.Focused += SliderAutoUnfocusEvent;
            sliderControlSlider.StyleClass = new List<string> { "AudioAttributeSlider" };

            sliderControlSlider.Minimum = numericSliderData.minVal;
            sliderControlSlider.Maximum = numericSliderData.maxVal;
            sliderControlSlider.Value = numericSliderData.defVal;
            sliderControlSlider.ValueChanged += _valueChangedEvent;
            sliderControlSlider.DragCompleted += sliderDragCompletedEvent;

            sliderControlEntry.WidthRequest = 100;
            sliderControlEntry.HeightRequest = 1;
            sliderControlEntry.Text = sliderControlSlider.Value.ToString($"n{numericSliderData.numDisplayDecPlaces}");
            sliderControlEntry.BackgroundColor = Color.FromArgb("#14141d");
            sliderControlEntry.TextColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
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

    #endregion

    #region Wave Shape Controls

    /// <summary>
    /// Oscillator Item UI Component WaveShape Control.
    /// </summary>
    internal class OscillatorItemWaveShapeControl : FlexLayout
    {
        ImageButton sineWaveBTN = new ImageButton();
        ImageButton squareWaveBTN = new ImageButton();
        ImageButton triangleWaveBTN = new ImageButton();

        /// <summary>
        /// OscillatorItemWaveShapeControl class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the oscillator UI Component).</param>
        public OscillatorItemWaveShapeControl(FlexLayout parentFLNode)
        {
            parentFLNode.SetGrow(this, 0.16f);
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;

            sineWaveBTN.Source = "sine_wave.png";
            sineWaveBTN.Padding = 15;
            sineWaveBTN.WidthRequest = 50;
            sineWaveBTN.HeightRequest = 50;
            sineWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            sineWaveBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
            sineWaveBTN.CornerRadius = 5;

            squareWaveBTN.Source = "digital_wave.png";
            squareWaveBTN.Padding = 15;
            squareWaveBTN.WidthRequest = 50;
            squareWaveBTN.HeightRequest = 50;
            squareWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            squareWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            squareWaveBTN.CornerRadius = 5;

            triangleWaveBTN.Source = "triangle_wave.png";
            triangleWaveBTN.Padding = 15;
            triangleWaveBTN.WidthRequest = 50;
            triangleWaveBTN.HeightRequest = 50;
            triangleWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            triangleWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            triangleWaveBTN.CornerRadius = 5;

            Children.Add(sineWaveBTN);
            Children.Add(squareWaveBTN);
            Children.Add(triangleWaveBTN);
        }
    }

    /// <summary>
    /// Oscillator Item UI Component control group SFX Button Area.
    /// </summary>
    internal class OscillatorItemSFXButtonArea : FlexLayout
    {
        private Button openSFXBTN = new Button();
        private EventHandler openSFXEditorEvent = delegate{};

        /// <summary>
        /// This property holds the function that runs when the user clicks on the Open SFX Button.
        /// </summary>
        public EventHandler OpenSFXEditorEvent {
            get
            {
                return openSFXEditorEvent;
            }
            set
            {
                openSFXBTN.Clicked -= openSFXEditorEvent;
                openSFXEditorEvent = value;
                openSFXBTN.Clicked += openSFXEditorEvent;
            }
        }

        /// <summary>
        /// OscillatorItemSFXButtonArea class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Oscillator Item UI Component).</param>
        public OscillatorItemSFXButtonArea(FlexLayout parentFLNode)
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;
            parentFLNode.SetGrow(this, 0.12f);

            openSFXBTN.Text = "Open SFX";
            openSFXBTN.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            openSFXBTN.FontAttributes = FontAttributes.Bold;
            openSFXBTN.FontFamily = "Orbitron";
            openSFXBTN.CornerRadius = 0;
            openSFXBTN.HeightRequest = 100;
            openSFXBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            this.SetGrow(openSFXBTN, 1.0f);

            Children.Add(openSFXBTN);
        }
    }

    /// <summary>
    /// Oscillator Item UI Component Wave Visualizer Area.
    /// </summary>
    internal class OscillatorItemWaveVizualizerArea : StackLayout
    {
        OscillatorItemWaveVizualizerGV visualizer;
        Microsoft.Maui.Dispatching.IDispatcherTimer updateFrameTimer;

        /// <summary>
        /// This property hold the function that updates the sound wave visualizer graph, every 0.033s (30 FPS).
        /// </summary>
        public IDispatcherTimer UpdateFrameTimer
        {
            get => updateFrameTimer;
            set => updateFrameTimer = value;
        }

        /// <summary>
        /// OscillatorItemWaveVizualizerArea class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Oscillator Item UI Component).</param>
        /// <param name="_graphUpdateFunction">Provides the function that updates the sound wave visualizer graph.</param>
        /// <param name="osc">Reference to audio system's oscillator engine object.</param>
        public OscillatorItemWaveVizualizerArea(FlexLayout parentFLNode, Func<float[]> _graphUpdateFunction, JuceAudioProvider osc)
        {
            parentFLNode.SetGrow(this, 0.16f);
            HeightRequest = 100;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = (Color)Application.Current.Resources["Gray950"];

            visualizer = new OscillatorItemWaveVizualizerGV(this, osc);

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
    /// Oscillator Item UI Component Wave Visualizer Graphics View.
    /// </summary>
    internal class OscillatorItemWaveVizualizerGV : GraphicsView
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
        /// OscillatorItemWaveVizualizerGV constructor method.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Oscillator Item UI Component).</param>
        /// <param name="osc">Reference to audio system's oscillator engine object.</param>
        public OscillatorItemWaveVizualizerGV(StackLayout parentFLNode, JuceAudioProvider osc)
        {
            HorizontalOptions = LayoutOptions.Fill;
            HeightRequest = 60;
            Drawable = drawableEngine;
            BackgroundColor = (Color)Application.Current.Resources["Gray500"];

            AP = osc;
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
        public float[] VisSamplesArray {
            get {
                return visSamplesArray;
            }
            set {
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

                    float y = midY - (visSamplesArray[i] * Amplitude * 50); // scale to view

                    if (i == 0)
                        path.MoveTo(x, y);
                    else
                        path.LineTo(x, y);
                }

                canvas.DrawPath(path);
            }
        }
    }

    #endregion
}