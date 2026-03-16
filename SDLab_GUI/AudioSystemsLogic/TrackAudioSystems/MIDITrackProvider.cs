using NAudio.Wave;
using SDLab_GUI.InteropWrapper;
using SDLab_GUI.UIComponents.TrackUIComponents;
using static SDLab_GUI.Global;

namespace SDLab_GUI.AudioSystemsLogic.TrackAudioSystems
{
    public class MIDITrackProvider : JuceAudioProvider
    {
        private bool currentRepeatMode = false;
        private float currentTempo = 1.0f;
        private AudioEngineMGMT AudioEngineMGMT;

        List<Global.struct_coordinates> activeNotes = new List<Global.struct_coordinates>();
        private int[,] pianoRollMatrix = new int[200, 6 * 12 + 1];
        private (int startTime, int noteIndex, int duration, float pitchRatio)[,] finalPianoRollMatrix = new (int startTime, int noteIndex, int duration, float pitchRatio)[200, 6 * 12 + 1];

        public List<Global.struct_coordinates> ActiveNotesLazyList { get => activeNotes; set => activeNotes = value; }
        public int[,] ActiveNotes { get => pianoRollMatrix; set => pianoRollMatrix = value; }

        public MIDITrackProvider(nint _engine, AudioEngineWrapper _engineBridgeRef, MainPage _mainPage, AudioEngineMGMT _audioManager, int _sampleRate, int _channels)
        {
            samplesPerBlock = 512;
            engine = _engine;
            engineBridgeRef = _engineBridgeRef;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channels);
            engineBridgeRef.EnginePrepareToPlay(engine, _sampleRate, samplesPerBlock);
            AudioEngineMGMT = _audioManager;
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

        public float renderMIDIWaveform(JuceAudioProvider templateSamplingProvider)
        {
            int maxValidNoteBlockColumnIndex = 0;
            int maxColumnNoteIndex = 6 * 12;

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 6 * 12 + 1; j++)
                {
                    (int startTime, int noteIndex, int duration, float pitchRatio) blankNoteInfo = (i, j, 0, 0);

                    finalPianoRollMatrix[i, j] = blankNoteInfo;

                    if (i > 0)
                    {
                        if (pianoRollMatrix[i, j] == 1 && pianoRollMatrix[i - 1, j] == 1)
                        {
                            continue;
                        }
                        else if (pianoRollMatrix[i, j] == 1 && pianoRollMatrix[i - 1, j] == 0)
                        {
                            int val = pianoRollMatrix[i, j], count = 0;
                            while (val == 1)
                            {
                                count++;
                                val = pianoRollMatrix[i + count, j];
                            }

                            (int startTime, int noteIndex, int duration, float pitchRatio) noteInfo = (i, j, count, (float)Math.Pow(2, ((double)j - 36) / 12));

                            finalPianoRollMatrix[i, j] = noteInfo;

                            if (i + count - 1 > maxValidNoteBlockColumnIndex) maxValidNoteBlockColumnIndex = i + count - 1;
                        }
                    }
                    else
                    {
                        if(pianoRollMatrix[i, j] == 1)
                        {
                            int val = pianoRollMatrix[i, j], count = 0;
                            while (val == 1)
                            {
                                count++;
                                val = pianoRollMatrix[i + count, j];
                            }

                            (int startTime, int noteIndex, int duration, float pitchRatio) noteInfo = (i, j, count, (float)Math.Pow(2, ((double)j - 36) / 12));

                            finalPianoRollMatrix[i, j] = noteInfo;

                            if (i + count - 1 > maxValidNoteBlockColumnIndex) maxValidNoteBlockColumnIndex = i + count - 1;
                        }
                    }
                }
            }

            engineBridgeRef._renderMIDIWaveform(engine, finalPianoRollMatrix, maxValidNoteBlockColumnIndex + 1, maxColumnNoteIndex + 1);

            return (maxValidNoteBlockColumnIndex + 1);
        }

        public void PlayMIDI()
        {
            AudioEngineMGMT.PlayMixer();
        }

        public void PauseMIDI()
        {
            AudioEngineMGMT.PauseMixer();
        }
    }
}