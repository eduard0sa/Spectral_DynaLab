using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.Configurations;
using SDLab_GUI.UIComponents.Editors;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    /// <summary>
    /// Oscillator Item UI Component.
    /// </summary>
    internal class OscillatorItem : TrackItem
    {
        private OscillatorItemWaveShapeControl _oscillatorItemWaveShapeControls;
        private OscillatorItemSFXButtonArea _oscillatorItemSFXButtonArea;

        private Global.structSliderData frequencySliderData;

        private bool activateWaveGraphMonitor = true;

        /// <summary>
        /// OscillatorItem class constructor.
        /// </summary>
        /// <param name="audioManager">Reference to the audio manager instance from MainPage.</param>
        /// <param name="mainPage">Reference from the Main Page program instance.</param>
        public OscillatorItem(AudioEngineMGMT audioManager, MainPage mainPage, float initialFrequency = 20, bool addToMixer = true, bool _activateWaveGraphMonitor = true)
        {
            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "OSCILLATOR", new List<string> { "YellowTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            _oscillatorItemWaveShapeControls = new OscillatorItemWaveShapeControl(this, setWaveShape);
            _oscillatorItemSFXButtonArea = new OscillatorItemSFXButtonArea(this);
            _oscillatorItemSFXButtonArea.OpenSFXEditorEvent = openSFXEditorEvent;

            trackAudioProvider = audioManager.LaunchAudioEngine(addToMixer: addToMixer);
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;
            activateWaveGraphMonitor = _activateWaveGraphMonitor;

            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, trackAudioProvider.pushOSCVisSampleArray, trackAudioProvider, activateWaveGraphMonitor);

            ((OscillatorAudioProvider)trackAudioProvider).CurrentFrequency = initialFrequency;

            frequencySliderData = new Global.structSliderData() {
                minVal = 20f,
                maxVal = 2000f,
                defVal = mainPageOBJ.EditorConfigurationSet._OscillatorSettigns.DefaultFrequency,
                numDisplayDecPlaces = 0
            };

            ((OscillatorAudioProvider)trackAudioProvider).changeFrequency(frequencySliderData.defVal);

            gainSliderData = new Global.structSliderData() {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = mainPageOBJ.EditorConfigurationSet._OscillatorSettigns.DefaultVolume,
                numDisplayDecPlaces = 2
            };

            trackAudioProvider.changeGain(gainSliderData.defVal);

            ((OscillatorAudioProvider)trackAudioProvider).changeWaveShapeFunction(mainPageOBJ.EditorConfigurationSet._OscillatorSettigns.DefaultWaveFormat);

            switch (mainPageOBJ.EditorConfigurationSet._OscillatorSettigns.DefaultWaveFormat)
            {
                case enumWaveShapeType.Sine:
                    _oscillatorItemWaveShapeControls.highlightButton(_oscillatorItemWaveShapeControls.SineWaveBTN);
                    break;
                case enumWaveShapeType.Square:
                    _oscillatorItemWaveShapeControls.highlightButton(_oscillatorItemWaveShapeControls.SquareWaveBTN);
                    break;
                case enumWaveShapeType.Triangle:
                    _oscillatorItemWaveShapeControls.highlightButton(_oscillatorItemWaveShapeControls.TriangleWaveBTN);
                    break;
            }

            UIPaint();
        }

        /// <summary>
        /// This method paints all the sub-components on the OscillatorItem UI Component.
        /// </summary>
        protected override void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);

            //Adding Slider Controls to the main Control Group.
            if ((mainPageOBJ.FindByName("trackStackLayout") as VerticalStackLayout).Children.Count == 1)
            {
                TrackItemControls[0].addSliderControl("Frequência:", enumBaseColor.YELLOW, frequencySliderData, frequencyChangeEvent, "oscillatorFrequency_0");
            }
            else
            {
                TrackItemControls[0].addSliderControl("Frequência:", enumBaseColor.YELLOW, frequencySliderData, frequencyChangeEvent);
            }

            TrackItemControls[0].addSliderControl("Ganho/Volume:", enumBaseColor.YELLOW, gainSliderData, gainChangeEvent);

            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);
            Children.Add(TrackItemControls[0]);
            Children.Add(_oscillatorItemWaveShapeControls);
            Children.Add(_oscillatorItemSFXButtonArea);
            Children.Add(TrackItemWaveVizualizerArea);
        }

        //==============================================Events==============================================

        /// <summary>
        /// This method changes the gain of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Gain Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void gainChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newGain = (float)e.NewValue;
            trackAudioProvider.changeGain(newGain);
        }

        /// <summary>
        /// This method changes the frequency of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Frequency Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void frequencyChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newFrequency = (float)e.NewValue;
            ((OscillatorAudioProvider)trackAudioProvider).changeFrequency(newFrequency);
        }

        /// <summary>
        /// This method deletes the oscillator from the UI and from the audio systems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            if (activateWaveGraphMonitor)
            {
                TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            }
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);

            mainPageOBJ.checkEmptyTrackListMessageVisibility();
        }

        /// <summary>
        /// This method opens the SFX Editor window when the user clicks on the proper UI button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSFXEditorEvent(object? sender, EventArgs e)
        {
            DSPModalEditor OscSFXDSPEditor = new DSPModalEditor(audioEngineMGMT, trackAudioProvider, mainPageOBJ);
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

        /// <summary>
        /// This method sets the oscillator wave shape to an option chosen by the user.
        /// </summary>
        /// <param name="sender">The option button clicked.</param>
        /// <param name="e">EventHandler's event arguments object.</param>
        private void setWaveShape(object? sender, EventArgs e)
        {
            ImageButton sourceIMGBTN = sender as ImageButton;

            switch (sourceIMGBTN.Source.ToString().Split(": ")[1])
            {
                case "sine_wave.png":
                    ((OscillatorAudioProvider)trackAudioProvider).changeWaveShapeFunction(Global.enumWaveShapeType.Sine);
                    break;
                case "digital_wave.png":
                    ((OscillatorAudioProvider)trackAudioProvider).changeWaveShapeFunction(Global.enumWaveShapeType.Square);
                    break;
                case "triangle_wave.png":
                    ((OscillatorAudioProvider)trackAudioProvider).changeWaveShapeFunction(Global.enumWaveShapeType.Triangle);
                    break;
            }

            (sourceIMGBTN.Parent as OscillatorItemWaveShapeControl).highlightButton(sourceIMGBTN);
        }
    }

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
        public OscillatorItemWaveShapeControl(FlexLayout parentFLNode, EventHandler waveTypeBTNClickEH)
        {
            parentFLNode.SetGrow(this, 0.16f);
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;

            SineWaveBTN.Source = "sine_wave.png";
            SineWaveBTN.Padding = 15;
            SineWaveBTN.WidthRequest = 50;
            SineWaveBTN.HeightRequest = 50;
            SineWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            SineWaveBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
            SineWaveBTN.CornerRadius = 5;
            SineWaveBTN.Clicked += waveTypeBTNClickEH;
            SineWaveBTN.AutomationId = "sineWaveFormatBTN";

            SquareWaveBTN.Source = "digital_wave.png";
            SquareWaveBTN.Padding = 15;
            SquareWaveBTN.WidthRequest = 50;
            SquareWaveBTN.HeightRequest = 50;
            SquareWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            SquareWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            SquareWaveBTN.CornerRadius = 5;
            SquareWaveBTN.Clicked += waveTypeBTNClickEH;
            SquareWaveBTN.AutomationId = "squareWaveFormatBTN";

            TriangleWaveBTN.Source = "triangle_wave.png";
            TriangleWaveBTN.Padding = 15;
            TriangleWaveBTN.WidthRequest = 50;
            TriangleWaveBTN.HeightRequest = 50;
            TriangleWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            TriangleWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            TriangleWaveBTN.CornerRadius = 5;
            TriangleWaveBTN.Clicked += waveTypeBTNClickEH;
            TriangleWaveBTN.AutomationId = "triangleWaveFormatBTN";

            Children.Add(SineWaveBTN);
            Children.Add(SquareWaveBTN);
            Children.Add(TriangleWaveBTN);
        }

        public ImageButton SineWaveBTN { get => sineWaveBTN; internal set => sineWaveBTN = value; }
        public ImageButton SquareWaveBTN { get => squareWaveBTN; internal set => squareWaveBTN = value; }
        public ImageButton TriangleWaveBTN { get => triangleWaveBTN; internal set => triangleWaveBTN = value; }

        public void highlightButton(ImageButton targetBTN)
        {
            SineWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            SquareWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            TriangleWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            targetBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
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

            openSFXBTN.Text = "Abrir SFX";
            openSFXBTN.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            openSFXBTN.FontAttributes = FontAttributes.Bold;
            openSFXBTN.FontFamily = "Orbitron";
            openSFXBTN.CornerRadius = 0;
            openSFXBTN.HeightRequest = 100;
            openSFXBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            openSFXBTN.TextColor = Colors.Black;
            this.SetGrow(openSFXBTN, 1.0f);

            Children.Add(openSFXBTN);
        }
    }

    #endregion
}