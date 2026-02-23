using NAudio.Wave;
using SDLab_InteropWrapper;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class MIDITrackProvider : JuceAudioProvider
    {
        private bool currentRepeatMode = false;
        private float currentTempo = 1.0f;

        public MIDITrackProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, int _sampleRate, int _channels)
        {
            samplesPerBlock = 512;
            engine = _engine;
            engineBridgeRef = _engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channels);
            engineBridgeRef.EnginePrepareToPlay(engine, _sampleRate, samplesPerBlock);
        }

        /// <summary>
        /// Sets the currentRepeatMode and updates the core audio engine with the specified repeatMode value.
        /// </summary>
        /// <param name="newRepeatState">The new gain value to set.</param>
        public void ChangeMIDITrackRepeatingMode(bool newRepeatState)
        {
            currentRepeatMode = newRepeatState;
            engineBridgeRef._changeMIDITrackRepeatingMode(engine, currentRepeatMode);
        }

        /// <summary>
        /// Sets the currentTempo and updates the core audio engine with the specified currentTempo value.
        /// </summary>
        /// <param name="newTempo">The new tempo value to set.</param>
        public void ChangeMIDITrackTempo(float newTempo)
        {
            currentTempo = newTempo;
            engineBridgeRef._changeMIDITrackTempo(engine, currentTempo);
        }
    }
}