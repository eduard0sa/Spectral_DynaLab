using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SDLab_InteropWrapper;

namespace SDLab_GUI
{
    internal class AudioEngineMGMT
    {
        private float defaultFrequency = 38.0f;
        private float defaultGain = 20f;

        public AudioEngineWrapper _AudioEngineRef = new AudioEngineWrapper();
        public IntPtr AudioEngineOsc;
        public List<JuceAudioProvider> oscillators = new List<JuceAudioProvider>();
        public MixingSampleProvider mixer;
        public VolumeSampleProvider vsp;
        public WaveOutEvent output = new WaveOutEvent();

        public AudioEngineMGMT()
        {
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
            {
                ReadFully = true
            };

            vsp = new VolumeSampleProvider(mixer);

            vsp.Volume = 1.0f;
        }

        public void initMixer()
        {
            output.Init(vsp);
        }

        public void PlayMixer()
        {
            output.Play();
        }

        public void PauseMixer()
        {
            output.Pause();
        }

        public JuceAudioProvider LaunchAudioEngine()
        {
            AudioEngineOsc = _AudioEngineRef.CreateEngine();
            JuceAudioProvider provider = new JuceAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

            provider.changeFrequency(defaultFrequency);
            provider.changeGain(defaultGain);

            mixer.AddMixerInput(provider);

            oscillators.Add(provider);
            return provider;
        }
    }

    internal class JuceAudioProvider : ISampleProvider
    {
        private readonly IntPtr engine;
        private readonly AudioEngineWrapper engineBridgeRef;
        private float currentFrequency;
        private float currentGain;

        public WaveFormat WaveFormat { get; }

        public IntPtr Engine { get => engine; }
        public float CurrentFrequency { get => currentFrequency; }
        public float CurrentGain { get => currentGain; }

        public JuceAudioProvider(IntPtr engine, AudioEngineWrapper engineBridgeRef, int sampleRate, int channels)
        {
            this.engine = engine;
            this.engineBridgeRef = engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            engineBridgeRef.EnginePrepareToPlay(engine, sampleRate, 512);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            engineBridgeRef.EngineProcessWave(Engine, buffer, count, offset);
            return count;
        }

        public void changeFrequency(float frequency)
        {
            currentFrequency = frequency;
            engineBridgeRef.ChangeFrequency(engine, frequency);
        }

        public void changeGain(float frequency)
        {
            currentGain = frequency;
            engineBridgeRef.ChangeGain(engine, frequency);
        }
    }
}