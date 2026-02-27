using NAudio.Wave;
using SDLab_InteropWrapper;
using SDLab_GUI.UIComponents.TrackUIComponents;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class MIDITrackProvider : JuceAudioProvider
    {
        private bool currentRepeatMode = false;
        private float currentTempo = 1.0f;
        private List<Global.struct_coordinates> activeNotes = new List<Global.struct_coordinates>();
        private Global.struct_coordinates hoveredNote;

        public List<Global.struct_coordinates> ActiveNotes { get => activeNotes; set => activeNotes = value; }

        public MIDITrackProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, MainPage _mainPage, AudioEngineMGMT _audioManager, int _sampleRate, int _channels)
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

        public void SetMIDITemplateSamplingProvider(JuceAudioProvider templateSamplingProvider)
        {
            engineBridgeRef._setMIDITemplateSamplingProvider(engine, templateSamplingProvider.Engine);
        }

        public void renderMIDIWaveform(JuceAudioProvider templateSamplingProvider)
        {
            if(ActiveNotes.Count > 0)
            {
                List<float> notesPitchRatios = new List<float>();

                for (int i = 0; i < ActiveNotes.Count; i++)
                {
                    notesPitchRatios.Add((float)Math.Pow(2, ((double)ActiveNotes[i].y - 46) / 12));
                }

                engineBridgeRef._renderMIDIWaveform(engine, notesPitchRatios.ToArray(), ActiveNotes.Count);
            }
            else
            {
                engineBridgeRef._renderMIDIWaveform(engine, new float[] { 0 }, 1);
            }
        }

        public void PlayMIDI()
        {

        }

        public void PauseMIDI()
        {

        }
    }
}