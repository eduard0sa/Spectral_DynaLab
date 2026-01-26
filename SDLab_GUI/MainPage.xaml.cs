using NAudio.Wave.SampleProviders;
using SDLab_InteropWrapper;

namespace SDLab_GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private AudioEngineMGMT audioManager;

        public MainPage()
        {
            InitializeComponent();

            AudioEngineWrapper audioEngineWrapper = new AudioEngineWrapper();
            audioManager = new AudioEngineMGMT();

            JuceAudioProvider provider2 = audioManager.LaunchAudioEngine();

            audioManager.initMixer();
            audioManager.PlayMixer();
            audioManager.PauseMixer();

            masterVolumeSlider.Value = audioManager.vsp.Volume * 100;
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value}%";
        }

        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        private void PlayPauseMixerEvent(object? sender, EventArgs e)
        {
            switch(mainPlayBTN.Source.ToString().Split(" ")[1])
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

        private void masterVolumeValueChangedEvent(object sender, ValueChangedEventArgs e)
        {
            Slider masterVolumeSlider = sender as Slider;
            audioManager.vsp.Volume = (float)(masterVolumeSlider.Value / 100);
            masterVolumeSliderValueLabel.Text = $"{masterVolumeSlider.Value}%";
        }
    }
}