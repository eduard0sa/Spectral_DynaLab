using SDLab_InteropWrapper;

namespace SpectralDynaLab_GUI
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

            audioManager.PlayMixer();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}