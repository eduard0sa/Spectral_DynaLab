using SDLab_InteropWrapper;

namespace SpectralDynaLab_GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private AudioEngineMGMT audioManager;

        public MainPage()
        {
            this.InitializeComponent();

            

            /*AudioEngineWrapper audioEngineWrapper = new AudioEngineWrapper();
            audioManager = new AudioEngineMGMT();

            JuceAudioProvider provider2 = audioManager.LaunchAudioEngine();

            audioManager.PlayMixer();*/
        }
    }
}