using NAudio.Wave;
using SDLab_InteropWrapper;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class FileTrackAudioProvider : JuceAudioProvider
    {
        private bool currentRepeatMode = false;

        public FileTrackAudioProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, int _sampleRate, int _channels)
        {
            samplesPerBlock = 512;
            engine = _engine;
            engineBridgeRef = _engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channels);
            engineBridgeRef.EnginePrepareToPlay(engine, _sampleRate, samplesPerBlock);
        }

        /// <summary>
        /// Sets the current gain and updates the core audio engine with the specified gain value.
        /// </summary>
        /// <param name="newRepeatState">The new gain value to set.</param>
        public void ChangeAudioFileRepeatingMode(bool newRepeatState)
        {
            currentRepeatMode = newRepeatState;
            engineBridgeRef.ChangeAudioFileRepeatingMode(engine, currentRepeatMode);
        }
    }
}