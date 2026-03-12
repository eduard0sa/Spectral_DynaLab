using NAudio.Wave;
using SDLab_GUI.InteropWrapper;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class OscillatorAudioProvider : JuceAudioProvider
    {
        private float currentFrequency;
        private enumWaveShapeType waveShapeType = enumWaveShapeType.Sine;

        public float CurrentFrequency { get => currentFrequency; set => currentFrequency = value; }
        public enumWaveShapeType WaveShapeType { get => waveShapeType; }

        public OscillatorAudioProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, int _sampleRate, int _channels)
        {
            samplesPerBlock = 512;
            engine = _engine;
            engineBridgeRef = _engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channels);
            engineBridgeRef.EnginePrepareToPlay(engine, _sampleRate, samplesPerBlock);
        }

        /// <summary>
        /// Sets the current frequency and updates the core audio engine with the specified frequency value.
        /// </summary>
        /// <param name="frequency">The new frequency value to set.</param>
        public void changeFrequency(float frequency)
        {
            currentFrequency = frequency;
            engineBridgeRef.ChangeFrequency(engine, frequency);
        }

        /// <summary>
        /// Sets the current waveShapeType and updates the core audio engine with the specified wave shape function.
        /// </summary>
        /// <param name="_waveShapeType"></param>
        public void changeWaveShapeFunction(enumWaveShapeType _waveShapeType)
        {
            waveShapeType = _waveShapeType;
            engineBridgeRef.ChangeWaveShapeFunction(engine, (int)_waveShapeType);
        }
    }
}