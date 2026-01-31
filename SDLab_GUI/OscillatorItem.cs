using SDLab_InteropWrapper;

namespace SDLab_GUI
{
    internal class OscillatorItem : FlexLayout
    {
        private JuceAudioProvider oscAudioProvider;
        private AudioEngineMGMT audioEngineMGMT;

        private OscillatorItemTriangleMark OscillatorTriangleMark;
        private OscillatorItemHeader OscillatorItemHeader;
        private OscillatorItemSliderControlGroup OscillatorItemSliderControls;
        private OscillatorItemWaveShapeControl OscillatorItemWaveShapeControls;
        private OscillatorItemSFXButtonArea OscillatorItemSFXButtonArea;
        private OscillatorItemWaveVizualizerArea OscillatorItemWaveVizualizerArea;

        private Global.structSliderData frequencySliderData;
        private Global.structSliderData gainSliderData;

        public OscillatorItem(AudioEngineMGMT audioManager)
        {
            OscillatorTriangleMark = new OscillatorItemTriangleMark(this, deleteOscillatorEvent);
            OscillatorItemHeader = new OscillatorItemHeader(this);
            OscillatorItemSliderControls = new OscillatorItemSliderControlGroup(this);
            OscillatorItemWaveShapeControls = new OscillatorItemWaveShapeControl(this);
            OscillatorItemSFXButtonArea = new OscillatorItemSFXButtonArea(this);
            OscillatorItemWaveVizualizerArea = new OscillatorItemWaveVizualizerArea(this);

            oscAudioProvider = audioManager.LaunchAudioEngine();
            audioEngineMGMT = audioManager;

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

        private void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);

            OscillatorItemSliderControls.addSliderControl("Frequency:", frequencySliderData, frequencyChangeEvent);
            OscillatorItemSliderControls.addSliderControl("Gain:", gainSliderData, gainChangeEvent);

            Children.Add(OscillatorTriangleMark);
            Children.Add(OscillatorItemHeader);
            Children.Add(OscillatorItemSliderControls);
            Children.Add(OscillatorItemWaveShapeControls);
            Children.Add(OscillatorItemSFXButtonArea);
            Children.Add(OscillatorItemWaveVizualizerArea);
        }

        //Events
        private void frequencyChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newFrequency = (float)(e.NewValue);
            oscAudioProvider.changeFrequency(newFrequency);
        }

        private void gainChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newGain = (float)(e.NewValue);
            oscAudioProvider.changeGain(newGain);
        }

        private void deleteOscillatorEvent(object? sender, EventArgs e)
        {
            audioEngineMGMT.removeAudioEngine(oscAudioProvider);
            (this.Parent as VerticalStackLayout).Children.Remove(this);
        }
    }

    #region Headers

    internal class OscillatorItemTriangleMark : FlexLayout
    {
        ImageButton triangleMarkBTN = new ImageButton();
        ImageButton deleteOscillatorBTN = new ImageButton();

        public OscillatorItemTriangleMark(FlexLayout parentFLNode, EventHandler clickEventHandler)
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
            deleteOscillatorBTN.Clicked += clickEventHandler;

            Children.Add(triangleMarkBTN);
            Children.Add(deleteOscillatorBTN);
        }
    }

    internal class OscillatorItemHeader : FlexLayout
    {
        Label HeaderLabel = new Label();
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
    internal class OscillatorItemSliderControlGroup : FlexLayout
    {
        List<OscillatorItemSliderControl> sliderControlList = new List<OscillatorItemSliderControl>();
        public OscillatorItemSliderControlGroup(FlexLayout parentFLNode)
        {
            Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;
            parentFLNode.SetGrow(this, 0.26f);
        }

        public void addSliderControl(string sliderLabel, Global.structSliderData numericSliderData, EventHandler<ValueChangedEventArgs> _valueChangedEvent)
        {
            OscillatorItemSliderControl sliderControl = new OscillatorItemSliderControl(sliderLabel, numericSliderData, _valueChangedEvent);

            sliderControlList.Add(sliderControl);

            updateSliderControlsFlexGrow();

            Children.Add(sliderControl);
        }

        private void updateSliderControlsFlexGrow()
        {
            for(int i = 0; i < sliderControlList.Count; i++)
            {
                this.SetGrow(sliderControlList[i], 1.0f / sliderControlList.Count);
            }
        }
    }

    internal class OscillatorItemSliderControl : FlexLayout
    {
        Label sliderControlLabel = new Label();
        Slider sliderControlSlider = new Slider();
        Entry sliderControlEntry = new Entry();
        Global.structSliderData _numericSliderData_;

        bool isUpdatingEntry = false;

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

    #endregion

    #region Wave Shape Controls

    internal class OscillatorItemWaveShapeControl : FlexLayout
    {
        ImageButton sineWaveBTN = new ImageButton();
        ImageButton squareWaveBTN = new ImageButton();
        ImageButton triangleWaveBTN = new ImageButton();

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

    internal class OscillatorItemSFXButtonArea : FlexLayout
    {
        Button openSFXBTN = new Button();
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
            openSFXBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
            this.SetGrow(openSFXBTN, 1.0f);

            Children.Add(openSFXBTN);
        }
    }

    internal class OscillatorItemWaveVizualizerArea : FlexLayout
    {
        OscillatorItemWaveVizualizerGV visualizer;
        public OscillatorItemWaveVizualizerArea(FlexLayout parentFLNode)
        {
            parentFLNode.SetGrow(this, 0.16f);
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            HeightRequest = 100;
            BackgroundColor = (Color)Application.Current.Resources["Gray100"];

            visualizer = new OscillatorItemWaveVizualizerGV(this);

            Children.Add(visualizer);
        }
    }

    internal class OscillatorItemWaveVizualizerGV : GraphicsView
    {
        SoundWaveShapeDrawable drawableEngine = new SoundWaveShapeDrawable();
        public OscillatorItemWaveVizualizerGV(FlexLayout parentFLNode)
        {
            parentFLNode.SetGrow(this, 0.90f);
            HeightRequest = 100;
            Drawable = drawableEngine;
            BackgroundColor = (Color)Application.Current.Resources["Gray500"];
        }

        public void updateWaveForm()
        {
            Invalidate();
        }
    }

    public class SoundWaveShapeDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            int Samples = 100;
            int Amplitude = 1;
            float Frequency = 1f; // in cycles over the width

            canvas.StrokeColor = (Color)Application.Current.Resources["DefaultRed"];
            canvas.StrokeSize = 5;

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
                float angle = t * MathF.PI * 2f * Frequency;

                float y =
                    midY -
                    MathF.Sin(angle) *
                    Amplitude *
                    (height * 0.4f); // scale to view

                if (i == 0)
                    path.MoveTo(x, y);
                else
                    path.LineTo(x, y);
            }

            canvas.DrawPath(path);
        }

        #endregion
    }
}