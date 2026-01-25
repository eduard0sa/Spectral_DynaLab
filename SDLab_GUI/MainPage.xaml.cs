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
        }

        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        private void PlayPauseMixerEvent(object? sender, EventArgs e)
        {
            switch(audioManager.output.PlaybackState)
            {
                case NAudio.Wave.PlaybackState.Paused:
                    audioManager.PlayMixer();
                    mainPlayBTN.Source = "play_solid_full.png";
                    break;

                case NAudio.Wave.PlaybackState.Playing:
                    audioManager.PauseMixer();
                    mainPlayBTN.Source = "pause_button.png";
                    break;
            }
            
        }
    }
}