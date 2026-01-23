using AUDIOPROCBRIDGE;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace OSC_UI
{
    internal class AudioEngineMGMT
    {
        private float defaultFrequency = 38.0f;
        private float defaultGain = 50f;

        public AudioEngineRef _AudioEngineRef = new AudioEngineRef();
        public IntPtr AudioEngineOsc;
        public List<JuceAudioProvider> oscillators = new List<JuceAudioProvider>();
        public MixingSampleProvider mixer;
        public WaveOutEvent output = new WaveOutEvent();

        public AudioEngineMGMT()
        {
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
            {
                ReadFully = true
            };
        }

        public void PlayMixer()
        {
            output.Init(mixer);
            output.Play();
        }

        public JuceAudioProvider LaunchAudioEngine()
        {
            AudioEngineOsc = _AudioEngineRef._createEngine();
            JuceAudioProvider provider = new JuceAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

            _AudioEngineRef._changeFrequency(provider.Engine, defaultFrequency);
            _AudioEngineRef._changeGain(provider.Engine, defaultGain);
            mixer.AddMixerInput(provider);

            oscillators.Add(provider);
            return provider;
        }
    }

    internal class JuceAudioProvider : ISampleProvider
    {
        private readonly IntPtr engine;
        private readonly AudioEngineRef engineBridgeRef;
        public WaveFormat WaveFormat { get; }

        public IntPtr Engine { get => engine; }

        public JuceAudioProvider(IntPtr engine, AudioEngineRef engineBridgeRef, int sampleRate, int channels)
        {
            this.engine = engine;
            this.engineBridgeRef = engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            engineBridgeRef._enginePrepareToPlay(engine, sampleRate, 512);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            engineBridgeRef._engineProcessWave(Engine, buffer, count, offset);
            return count;
        }
    }
}