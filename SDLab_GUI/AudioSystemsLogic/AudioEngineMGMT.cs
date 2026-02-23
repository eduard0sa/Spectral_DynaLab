using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_InteropWrapper;

namespace SDLab_GUI.AudioSystemsLogic
{
    /// <summary>
    /// <para> Audio engine management class. </para>
    /// <para> It handles sound entries and effects and contacts core audio engine through the Interop Layer. </para>
    /// </summary>
    public class AudioEngineMGMT
    {
        private float defaultFrequency = 38.0f;
        private float defaultGain = 0.5f;

        public AudioEngineWrapper _AudioEngineRef = new AudioEngineWrapper();
        public nint AudioEngineOsc;
        public List<JuceAudioProvider> oscillators = new List<JuceAudioProvider>();
        public MixingSampleProvider mixer;
        public VolumeSampleProvider vsp;
        public WaveOutEvent output = new WaveOutEvent();

        public AudioEngineMGMT()
        {
            //Initializing Mixer Provider
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
            {
                ReadFully = true
            };

            //Initializing Volume Sample Provider
            vsp = new VolumeSampleProvider(mixer);
            vsp.Volume = 1.0f;
        }

        /// <summary>
        /// Initializes the audio mixer with the specified audio source.
        /// </summary>
        public void initMixer()
        {
            output.Init(vsp);
        }

        /// <summary>
        /// Starts playback of the audio mixer output.
        /// </summary>
        public void PlayMixer()
        {
            output.Play();
        }

        /// <summary>
        /// Pauses audio playback on the mixer output.
        /// </summary>
        public void PauseMixer()
        {
            output.Pause();
        }

        /// <summary>
        /// Removes a specified audio engine and all its associated DSP effects from the app and from audio engines.
        /// </summary>
        /// <param name="provider">The JuceAudioProvider instance representing the audio engine to remove.</param>
        public void removeAudioEngine(JuceAudioProvider provider)
        {
            //Associated DSPs removal
            int dspProcessorsCount = (int)provider.DspProcessors.Count;
            int counter = 0;

            while(counter < dspProcessorsCount)
            {
                provider.removeDSPEffect(provider.DspProcessors[0]);
                counter++;
            }

            //Oscillator removal
            mixer.RemoveMixerInput(provider);
            oscillators.Remove(provider);
            _AudioEngineRef.DestroyEngine(provider.Engine);
        }

        /// <summary>
        /// Initializes a new JuceAudioProvider with default frequency and gain, adds it to the mixer and oscillator list, and returns the provider.
        /// </summary>
        /// <returns>The newly created and configured JuceAudioProvider instance.</returns>
        public JuceAudioProvider LaunchAudioEngine(bool isMIDI = false)
        {
            if (!isMIDI)
            {
                //Oscillator instantiation in the core audio engine
                AudioEngineOsc = _AudioEngineRef.CreateEngine();
                OscillatorAudioProvider provider = new OscillatorAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

                provider.changeFrequency(defaultFrequency);
                provider.changeGain(defaultGain);

                //New oscillator instance append in mixer
                mixer.AddMixerInput(provider);

                oscillators.Add(provider);
                return provider;
            }
            else
            {
                //MIDI TRACK instantiation in the core audio engine
                AudioEngineOsc = _AudioEngineRef.CreateEngine(true);
                MIDITrackProvider provider = new MIDITrackProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

                provider.changeGain(defaultGain);

                //New oscillator instance append in mixer
                mixer.AddMixerInput(provider);

                oscillators.Add(provider);
                return provider;
            }
        }

        /// <summary>
        /// Initializes a new JuceAudioProvider with default frequency and gain, adds it to the mixer and oscillator list, and returns the provider.
        /// </summary>
        /// <returns>The newly created and configured JuceAudioProvider instance.</returns>
        public JuceAudioProvider LaunchAudioEngine(string musicPath)
        {
            //Oscillator instantiation in the core audio engine
            AudioEngineOsc = _AudioEngineRef.CreateEngine(musicPath);
            FileTrackAudioProvider provider = new FileTrackAudioProvider(AudioEngineOsc, _AudioEngineRef, 44100, 1);

            provider.changeGain(defaultGain);

            //New oscillator instance append in mixer
            mixer.AddMixerInput(provider);

            oscillators.Add(provider);
            return provider;
        }
    }
}