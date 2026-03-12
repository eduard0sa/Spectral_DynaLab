using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.Editors;
using Windows.Networking.Vpn;
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
                defVal = ((OscillatorAudioProvider)trackAudioProvider).CurrentFrequency,
                numDisplayDecPlaces = 0
            };

            gainSliderData = new Global.structSliderData() {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = trackAudioProvider.CurrentGain,
                numDisplayDecPlaces = 2
            };

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
            TrackItemControls[0].addSliderControl("Frequency:", enumBaseColor.YELLOW, frequencySliderData, frequencyChangeEvent);
            TrackItemControls[0].addSliderControl("Gain:", enumBaseColor.YELLOW, gainSliderData, gainChangeEvent);

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
        }

        /// <summary>
        /// This method opens the SFX Editor window when the user clicks on the proper UI button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSFXEditorEvent(object? sender, EventArgs e)
        {
            DSPModalEditor OscSFXDSPEditor = new DSPModalEditor(audioEngineMGMT, trackAudioProvider);
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

            sineWaveBTN.Source = "sine_wave.png";
            sineWaveBTN.Padding = 15;
            sineWaveBTN.WidthRequest = 50;
            sineWaveBTN.HeightRequest = 50;
            sineWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            sineWaveBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelYellow"];
            sineWaveBTN.CornerRadius = 5;
            sineWaveBTN.Clicked += waveTypeBTNClickEH;

            squareWaveBTN.Source = "digital_wave.png";
            squareWaveBTN.Padding = 15;
            squareWaveBTN.WidthRequest = 50;
            squareWaveBTN.HeightRequest = 50;
            squareWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            squareWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            squareWaveBTN.CornerRadius = 5;
            squareWaveBTN.Clicked += waveTypeBTNClickEH;

            triangleWaveBTN.Source = "triangle_wave.png";
            triangleWaveBTN.Padding = 15;
            triangleWaveBTN.WidthRequest = 50;
            triangleWaveBTN.HeightRequest = 50;
            triangleWaveBTN.Margin = new Thickness(0, 0, 5, 0);
            triangleWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            triangleWaveBTN.CornerRadius = 5;
            triangleWaveBTN.Clicked += waveTypeBTNClickEH;

            Children.Add(sineWaveBTN);
            Children.Add(squareWaveBTN);
            Children.Add(triangleWaveBTN);
        }

        public void highlightButton(ImageButton targetBTN)
        {
            sineWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            squareWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
            triangleWaveBTN.BackgroundColor = Color.FromArgb("#21232a");
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
            this.SetGrow(openSFXBTN, 1.0f);

            Children.Add(openSFXBTN);
        }
    }

    #endregion
}