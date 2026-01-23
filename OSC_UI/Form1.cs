using AUDIOPROCBRIDGE;

namespace OSC_UI
{
    public partial class Form1 : Form
    {
        private AudioEngineMGMT audioManager;

        public Form1()
        {
            InitializeComponent();
            audioManager = new AudioEngineMGMT();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JuceAudioProvider provider2 = audioManager.LaunchAudioEngine();

            audioManager.PlayMixer();

            AddOscilatorUIEntity(provider2, audioManager._AudioEngineRef);
        }

        private void formClose(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void AddOscilatorUIEntity(JuceAudioProvider oscEngine, AudioEngineRef _audioEngineRef)
        {
            OscillatorItem _oscillatorItem = new OscillatorItem(oscEngine, _audioEngineRef);
            this.OscillatorsListPanel.Controls.Add(_oscillatorItem);
        }
    }
}