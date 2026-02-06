using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.UIComponents;
using SDLab_InteropWrapper;

namespace SDLab_GUI
{
    public partial class MainPage : ContentPage
    {
        private AudioEngineMGMT audioManager;

        bool isUpdatingMasterVolumeSlider = false;

        public MainPage()
        {
            InitializeComponent();

            AudioEngineWrapper audioEngineWrapper = new AudioEngineWrapper();
            audioManager = new AudioEngineMGMT();

            audioManager.initMixer();

            masterVolumeSlider.Value = audioManager.vsp.Volume * 100;
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value}%";
        }

        /*protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Window_Destroying();
        }

        public void Window_Destroying()
        {
            for(int i = 0; i < audioManager.oscillators.Count; i++)
            {
                audioManager.removeAudioEngine(audioManager.oscillators[i]);
            }
        }*/

        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

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

        private void masterVolumeSliderValueChangedEvent(object sender, ValueChangedEventArgs e)
        {
            Slider masterVolumeSlider = sender as Slider;
            audioManager.vsp.Volume = (float)(masterVolumeSlider.Value / 100);
        }

        private void masterVolumeSliderDragCompletedEvent(object sender, EventArgs e)
        {
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value.ToString("n2")}%";
            isUpdatingMasterVolumeSlider = true;
        }

        private void masterVolumeEntryValueChangedEvent(object sender, TextChangedEventArgs e)
        {
            Entry masterVolumeEntry = sender as Entry;
            if(masterVolumeEntry.Text.Length >= 2)
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

        private void addOscillatorBTNClickedEvent(object sender, EventArgs e)
        {
            OscillatorItem newOscillator = new OscillatorItem(audioManager, this);

            trackStackLayout.Children.Add(newOscillator);
        }
    }
}