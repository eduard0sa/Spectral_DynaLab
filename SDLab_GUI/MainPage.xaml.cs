using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.InteropWrapper;
using SDLab_GUI.Tutorials;
using SDLab_GUI.UIComponents.Editors;
using SDLab_GUI.UIComponents.TrackUIComponents;
using System.Windows.Input;
using static SDLab_GUI.Global;

namespace SDLab_GUI
{
    public enum enumEditorMode
    {
        Default,
        Tutorial
    }

    /// <summary>
    /// Editor Suite Content Page
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private AudioEngineMGMT audioManager;
        private bool isUpdatingMasterVolumeSlider = false;

        private ICommand midiTrackBTNClickCommand;
        private ICommand fileTrackBTNClickCommand;
        private ICommand oscillatorTrackBTNClickCommand;

        private LoadingModalView newLoadingModal = new LoadingModalView();
        private TutorialModalOverlayView editorTutorial;

        private IDispatcherTimer highlightAnimationTimer;
        private int zoomDirection = -1;

        public ICommand MIDITrackBTNClickCommand { get => midiTrackBTNClickCommand; set => midiTrackBTNClickCommand = value; }

        public MainPage(enumEditorMode runMode)
        {
            //Initialize UI
            InitializeComponent();

            //Initiallize audio Engine/Mixer
            AudioEngineWrapper audioEngineWrapper = new AudioEngineWrapper();
            audioManager = new AudioEngineMGMT(this);

            audioManager.initMixer();

            masterVolumeSlider.Value = audioManager.vsp.Volume * 100;
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value}%";

            midiTrackBTNClickCommand = new Command(addMIDITrackBTNClickedEvent);
            fileTrackBTNClickCommand = new Command(addFileTrackBTNClickedEvent);
            oscillatorTrackBTNClickCommand = new Command(addOscillatorBTNClickedEvent);

            addMidiTrackBTN.Command = MIDITrackBTNClickCommand;
            addFileTrackBTN.Command = fileTrackBTNClickCommand;
            addOscillatorTrackBTN.Command = oscillatorTrackBTNClickCommand;

            if (runMode == enumEditorMode.Tutorial) showTutorialOverlay();
        }

        #region PageNavigation

        /// <summary>
        /// This method is attached to the "Save Project" button in the editor interface. It handles mixing set save operations, saving the project information into an XML-Formated file.
        /// </summary>
        /// <param name="sender">The source save button.</param>
        /// <param name="e">Event Handler's Event Args Object.</param>
        private void saveProjectChangesBTNClickedEvent(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method navigates the user interface to the start page, popping the Editor from the Navigation Stack.
        /// </summary>
        /// <param name="sender">The source save button.</param>
        /// <param name="e">Event Handler's Event Args Object.</param>
        private void closeEditorBTNClickedEvent(object sender, EventArgs e)
        {
            if (mainPlayBTN.Source.ToString().Split(" ")[1] == "pause_button.png")
            {
                PlayPauseExternalWrapper();
            }

            Navigation.PopAsync();
        }

        #endregion PageNavigation

        #region LoadingScreen

        /// <summary>
        /// This method shows a translucent loading screen, providing the user a visual feedback that the application is processing a certain operation.
        /// </summary>
        public void ShowLoadingModalSplashScreen()
        {
            newLoadingModal = new LoadingModalView();
            Navigation.PushModalAsync(newLoadingModal);
        }

        /// <summary>
        /// This method hides the loading screen, when the application finishes processing the operation that triggered the loading screen.
        /// </summary>
        public void HideLoadingModalSplashScreen()
        {
            Navigation.PopModalAsync();
        }

        #endregion LoadingScreen

        #region MusicControl

        /// <summary>
        /// This method automatically unfocuses a slider element when it is focus, in order to avoid it from focusing on app start.
        /// </summary>
        /// <param name="sender">Slider sender object.</param>
        /// <param name="e">Slider focus EventArguments.</param>
        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        /// <summary>
        /// This method is attached to the play/pause button at the top of the suite interface.
        /// It tells the audio mixer to pause or play music.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPauseMixerEvent(object? sender, EventArgs e)
        {
            switch (mainPlayBTN.Source.ToString().Split(" ")[1])
            {
                case "pause_button.png":
                    audioManager.PauseMixer();
                    mainPlayBTN.Source = "play_solid_full.png";
                    break;

                case "play_solid_full.png":
                    audioManager.PlayMixer();
                    mainPlayBTN.Source = "pause_button.png";
                    break;
            }
        }

        /// <summary>
        /// This method wraps the PlayPauseMixerEvent method, allowing external classes to trigger play/pause method without providing event parameters.
        /// </summary>
        public void PlayPauseExternalWrapper()
        {
            PlayPauseMixerEvent(new object(), new EventArgs());
        }

        /// <summary>
        /// Handles volume changes to the master volume slider and updates the audio manager's volume accordingly.
        /// </summary>
        /// <param name="sender">The slider control that triggered the event.</param>
        /// <param name="e">Slider value change EventArguments.</param>
        private void masterVolumeSliderValueChangedEvent(object sender, ValueChangedEventArgs e)
        {
            Slider masterVolumeSlider = sender as Slider;
            audioManager.vsp.Volume = (float)(masterVolumeSlider.Value / 100);
        }

        /// <summary>
        /// Handles the completion of a drag event on the master volume slider by updating the value label and setting the update flag.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void masterVolumeSliderDragCompletedEvent(object sender, EventArgs e)
        {
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value.ToString("n2")}%";
            isUpdatingMasterVolumeSlider = true;
        }

        /// <summary>
        /// Handles changes to the master volume entry field, updating the audio volume, slider, and label accordingly.
        /// </summary>
        /// <param name="sender">The Entry control whose text was changed.</param>
        /// <param name="e">Event data for the text changed event.</param>
        private void masterVolumeEntryValueChangedEvent(object sender, TextChangedEventArgs e)
        {
            Entry masterVolumeEntry = sender as Entry;
            if (masterVolumeEntry.Text.Length >= 2)
            {
                if (float.TryParse(masterVolumeEntry.Text.Substring(0, masterVolumeEntry.Text.Length - 2), out float newVolume) && isUpdatingMasterVolumeSlider == false)
                {
                    if (newVolume >= 0 && newVolume <= 100)
                    {
                        audioManager.vsp.Volume = newVolume / 100;
                        masterVolumeSlider.Value = newVolume;
                        masterVolumeSliderValueLabel.Text = $"{newVolume.ToString("n2")}%";
                    }
                }
                else
                {
                    isUpdatingMasterVolumeSlider = false;
                }
            }
        }

        #endregion MusicControl

        #region TrackProvidersAppend

        /// <summary>
        /// Handles the event when the add oscillator button is clicked by creating a new OscillatorItem and adding it to the track stack layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void addOscillatorBTNClickedEvent()
        {
            OscillatorItem newOscillator = new OscillatorItem(audioManager, this);

            trackStackLayout.Children.Add(newOscillator);
        }

        /// <summary>
        /// Handles the event when the add audio file track button is clicked by creating a new FileTrackItem and adding it to the track stack layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void addFileTrackBTNClickedEvent()
        {
            var FileChoiceDialog = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecionar ficheiro de audio (.mp3, .wav, .ogg, etc...)."
            });

            if (FileChoiceDialog == null) return;

            FileTrackItem newOscillator = new FileTrackItem(audioManager, this, FileChoiceDialog.FullPath);

            trackStackLayout.Children.Add(newOscillator);
        }

        /// <summary>
        /// Handles the event when the add oscillator button is clicked by creating a new OscillatorItem and adding it to the track stack layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void addMIDITrackBTNClickedEvent()
        {
            MIDITrackItem newMIDITrack = new MIDITrackItem(audioManager, this);

            trackStackLayout.Children.Add(newMIDITrack);
        }

        #endregion TrackProvidersAppend

        #region TutorialSystem

        /// <summary>
        /// This method shows the tutorial overlay, effectively starting up the dynamic tutorial system.
        /// </summary>
        private void showTutorialOverlay()
        {
            editorTutorial = new TutorialModalOverlayView(this);
            Navigation.PushModalAsync(editorTutorial);
        }

        /// <summary>
        /// This method hides the tutorial overlay, effectively finishing and closing the dynamic tutorial system.
        /// </summary>
        public void closeTutorialOverlay()
        {
            if (editorTutorial != null)
            {
                Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// This method highlights a button in the editor, when the tutorial system requests.
        /// </summary>
        /// <param name="_btnName"></param>
        /// <returns></returns>
        public struct_elementBoundInfo highlightBTN(string _btnName)
        {
            Button targetButton = ((Button)FindByName(_btnName));

            if(targetButton != null)
            {
                highlightAnimationTimer = Dispatcher.CreateTimer();
                highlightAnimationTimer.Interval = TimeSpan.FromMilliseconds(1000 / 20f);
                highlightAnimationTimer.Tick += delegate
                {
                    if (targetButton.BackgroundColor.Alpha <= 0)
                    {
                        zoomDirection = 1;
                    }
                    if (targetButton.BackgroundColor.Alpha >= 1)
                    {
                        zoomDirection = -1;
                    }

                    targetButton.BackgroundColor = targetButton.BackgroundColor.WithAlpha(targetButton.BackgroundColor.Alpha + (0.1f * zoomDirection));
                };

                highlightAnimationTimer.Start();

                struct_elementBoundInfo elementBoundInfo = new struct_elementBoundInfo
                {
                    Bounds = targetButton.Bounds,
                    sourceElement = targetButton
                };

                return elementBoundInfo;
            }

            return new struct_elementBoundInfo();
        }

        /// <summary>
        /// This method stops the highlight animation and restores the background opacity of the specified button.
        /// </summary>
        /// <param name="_btnName">The name of the button to stop highlighting.</param>
        public void stopBTNHighlight(string _btnName)
        {
            highlightAnimationTimer.Stop();

            Button targetButton = ((Button)FindByName(_btnName));

            if(targetButton != null)
            {
                targetButton.BackgroundColor = targetButton.BackgroundColor.WithAlpha(1.0f);
            }
        }

        #endregion TutorialSystem
    }
}