using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.Editors;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    internal class FileTrackItem : TrackItem
    {
        private string musicPath;
        private bool addToMixer, activateWaveGraphMonitor;

        private OscillatorItemSFXButtonArea sfxButton;
        private structSwitchData repeatTrackModeSwitchData;
        private structSliderData tempoSliderData;
        private structSliderData pitchSliderData;
        private structSwitchData timePitchCouplingModeSwitchData;

        public FileTrackItem(AudioEngineMGMT audioManager, MainPage mainPage, string _musicPath, bool _addToMixer = true, bool _activateWaveGraphMonitor = true)
        {
            sfxButton = new OscillatorItemSFXButtonArea(this);
            sfxButton.OpenSFXEditorEvent = openSFXEditorEvent;

            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "FILE TRACK", new List<string> { "GreenTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));

            trackAudioProvider = audioManager.LaunchAudioEngine(_musicPath, addToMixer: _addToMixer);
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;
            musicPath = _musicPath;
            addToMixer = _addToMixer;
            activateWaveGraphMonitor = _activateWaveGraphMonitor;

            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, trackAudioProvider.pushOSCVisSampleArray, trackAudioProvider);

            gainSliderData = new Global.structSliderData()
            {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = trackAudioProvider.CurrentGain,
                numDisplayDecPlaces = 2
            };

            repeatTrackModeSwitchData = new structSwitchData()
            {
                defValIndex = false
            };

            tempoSliderData = new structSliderData() {
                minVal = 0.2f,
                maxVal = 5.0f,
                defVal = 1.0f,
                numDisplayDecPlaces = 2
            };

            pitchSliderData = new structSliderData() {
                minVal = 0.1f,
                maxVal = 5.0f,
                defVal = 1.0f,
                numDisplayDecPlaces = 2
            };

            timePitchCouplingModeSwitchData = new structSwitchData()
            {
                defValIndex = true
            };

            UIPaint();
        }

        protected override void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);

            //Adding Slider Controls to the main Control Group.
            TrackItemControls[0].addSliderControl("Gain:", enumBaseColor.GREEN, gainSliderData, gainChangeEvent);
            TrackItemControls[0].addSwitchControl("Repetir faixa:", enumBaseColor.GREEN, repeatTrackModeSwitchData, repeatModeChangeEvent);
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            TrackItemControls[1].addSliderControl("Tempo:", enumBaseColor.GREEN, tempoSliderData, tempoChangeEvent);
            TrackItemControls[1].addSliderControl("Pitch:", enumBaseColor.GREEN, pitchSliderData, pitchChangeEvent);
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            TrackItemControls[2].addSwitchControl("Ligação Time-Pitch", enumBaseColor.GREEN, timePitchCouplingModeSwitchData, TimePitchCouplingChangeEvent);

            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);
            if (addToMixer)
            {
                Children.Add(TrackItemControls[0]);
            }
            Children.Add(TrackItemControls[1]);
            Children.Add(TrackItemControls[2]);
            Children.Add(sfxButton);
            Children.Add(TrackItemWaveVizualizerArea);

            IDispatcherTimer timerToHideTempoSlider = Dispatcher.CreateTimer();

            timerToHideTempoSlider.Interval = TimeSpan.FromMilliseconds(50);
            timerToHideTempoSlider.IsRepeating = false;

            timerToHideTempoSlider.Tick += delegate {
                TrackItemControls[1].toggleControlVisibility(0);
                timerToHideTempoSlider.Stop();
            };

            timerToHideTempoSlider.Start();
        }

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
        /// This method changes the repeatMode of the file track, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin RepeatMode Switch.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void repeatModeChangeEvent(object? sender, ToggledEventArgs e)
        {
            bool newRepeatState = (bool)e.Value;
            ((FileTrackAudioProvider)trackAudioProvider).ChangeAudioFileRepeatingMode(newRepeatState);
        }

        /// <summary>
        /// This method changes the Time-Pitch Coupling Mode of the file track, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Time-Pitch Coupling Mode Switch.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void TimePitchCouplingChangeEvent(object? sender, ToggledEventArgs e)
        {
            bool newTimePitchCouplingState = (bool)e.Value;
            TrackItemControls[1].toggleControlVisibility(0);
            ((FileTrackAudioProvider)trackAudioProvider).ChangeAudioFileTimePitchCouplingMode(newTimePitchCouplingState);
        }

        /// <summary>
        /// This method changes the gain of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Gain Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void tempoChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newTempo = (float)e.NewValue;
            ((FileTrackAudioProvider)trackAudioProvider).ChangeAudioFileTempo(newTempo);
        }

        /// <summary>
        /// This method changes the gain of the oscillator, on the UI component and in the audio systems.
        /// </summary>
        /// <param name="sender">The origin Gain Slider.</param>
        /// <param name="e">Event Handler Event Arguments data.</param>
        private void pitchChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newPitch = (float)e.NewValue;
            ((FileTrackAudioProvider)trackAudioProvider).ChangeAudioFilePitch(newPitch);
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

        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);

            mainPageOBJ.checkEmptyTrackListMessageVisibility();
        }
    }
}