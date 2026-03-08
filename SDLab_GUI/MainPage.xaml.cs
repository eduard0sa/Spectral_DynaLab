using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.InteropWrapper;
using SDLab_GUI.Tutorials;
using SDLab_GUI.UIComponents.Editors;
using SDLab_GUI.UIComponents.TrackUIComponents;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer.DragDrop.Core;
using Windows.ApplicationModel.VoiceCommands;
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

        private LoadingModalView newLoadingModal = new LoadingModalView();
        private TutorialModalOverlayView editorTutorial;

        private IDispatcherTimer highlightAnimationTimer;
        private int zoomDirection = -1;

        public ICommand MIDITrackBTNClickCommand { get => midiTrackBTNClickCommand; }

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

            if (runMode == enumEditorMode.Tutorial)
            {
                editorTutorial = new TutorialModalOverlayView(this);
                Navigation.PushModalAsync(editorTutorial);
            }
        }

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

        public void ShowLoadingModalSplashScreen()
        {
            newLoadingModal = new LoadingModalView();
            Navigation.PushModalAsync(newLoadingModal);
        }

        public void HideLoadingModalSplashScreen()
        {
            Navigation.PopModalAsync();
        }

        /// <summary>
        /// Handles the event when the add oscillator button is clicked by creating a new OscillatorItem and adding it to the track stack layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void addOscillatorBTNClickedEvent(object sender, EventArgs e)
        {
            OscillatorItem newOscillator = new OscillatorItem(audioManager, this);

            trackStackLayout.Children.Add(newOscillator);
        }

        /// <summary>
        /// Handles the event when the add audio file track button is clicked by creating a new FileTrackItem and adding it to the track stack layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void addFileTrackBTNClickedEvent(object sender, EventArgs e)
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

        private void saveProjectChangesBTNClickedEvent(object sender, EventArgs e)
        {

        }

        private void closeEditorBTNClickedEvent(object sender, EventArgs e)
        {
            if (mainPlayBTN.Source.ToString().Split(" ")[1] == "pause_button.png")
            {
                PlayPauseExternalWrapper();
            }

            Navigation.PopAsync();
        }

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

        public void stopBTNHighlight(string _btnName)
        {
            highlightAnimationTimer.Stop();

            Button targetButton = ((Button)FindByName(_btnName));

            if(targetButton != null)
            {
                targetButton.BackgroundColor = targetButton.BackgroundColor.WithAlpha(1.0f);
            }
        }

        public void closeTutorialOverlay()
        {
            if (editorTutorial != null)
            {
                Navigation.PopModalAsync();
            }
        }
    }
}