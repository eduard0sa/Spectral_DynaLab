using NAudio.Wave;
using SDLab_GUI.InteropWrapper;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class FileTrackAudioProvider : JuceAudioProvider
    {
        private bool currentRepeatMode = false;
        private bool currentTimePitchCouplingMode = false;
        private float currentTempo = 1.0f;
        private float currentPitch = 1.0f;

        public FileTrackAudioProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, int _sampleRate, int _channels)
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
        public void ChangeAudioFileRepeatingMode(bool newRepeatState)
        {
            currentRepeatMode = newRepeatState;
            engineBridgeRef.ChangeAudioFileRepeatingMode(engine, currentRepeatMode);
        }

        /// <summary>
        /// Sets the currentTimePitchCouplingMode and updates the core audio engine with the specified currentTimePitchCouplingMode value.
        /// </summary>
        /// <param name="newTimePitchCouplingState">The new Time Pitch Coupling Mode value to set.</param>
        public void ChangeAudioFileTimePitchCouplingMode(bool newTimePitchCouplingState)
        {
            currentTimePitchCouplingMode = newTimePitchCouplingState;
            engineBridgeRef.ChangeAudioFileTimePitchCouplingMode(engine, currentTimePitchCouplingMode);
        }

        /// <summary>
        /// Sets the currentTempo and updates the core audio engine with the specified currentTempo value.
        /// </summary>
        /// <param name="newTempo">The new tempo value to set.</param>
        public void ChangeAudioFileTempo(float newTempo)
        {
            currentTempo = newTempo;
            engineBridgeRef.ChangeAudioFileTempo(engine, currentTempo);
        }

        /// <summary>
        /// Sets the currentPitch and updates the core audio engine with the specified currentPitch value.
        /// </summary>
        /// <param name="newPitch">The new pitch value to set.</param>
        public void ChangeAudioFilePitch(float newPitch)
        {
            currentPitch = newPitch;
            engineBridgeRef.ChangeAudioFilePitch(engine, currentPitch);
        }
    }
}