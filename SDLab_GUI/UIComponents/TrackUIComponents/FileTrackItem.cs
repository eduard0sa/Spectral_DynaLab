using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.Editors;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    internal class FileTrackItem : TrackItem
    {
        private string musicPath;
        OscillatorItemSFXButtonArea sfxButton;

        public FileTrackItem(AudioEngineMGMT audioManager, MainPage mainPage, string _musicPath)
        {
            sfxButton = new OscillatorItemSFXButtonArea(this);
            sfxButton.OpenSFXEditorEvent = openSFXEditorEvent;

            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "FILE TRACK", new List<string> { "GreenTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));

            trackAudioProvider = audioManager.LaunchAudioEngine(_musicPath);
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;
            musicPath = _musicPath;

            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, trackAudioProvider.pushOSCVisSampleArray, trackAudioProvider);

            gainSliderData = new Global.structSliderData()
            {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = trackAudioProvider.CurrentGain,
                numDisplayDecPlaces = 2
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
            TrackItemControls[0].addSwitchControl("Repeat Track:", enumBaseColor.GREEN, gainSliderData, repeatModeChangeEvent);

            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);
            Children.Add(TrackItemControls[0]);
            Children.Add(sfxButton);
            Children.Add(TrackItemWaveVizualizerArea);
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

        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);
        }
    }
}