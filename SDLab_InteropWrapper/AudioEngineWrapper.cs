using AUDIOPROCBRIDGE;

namespace SDLab_InteropWrapper
{
    public class AudioEngineWrapper
    {
        AudioEngineRef audioEngineRef;

        public AudioEngineWrapper()
        {
            audioEngineRef = new AudioEngineRef();
        }

        public int test()
        {
            return 3301;
        }

        public IntPtr CreateEngine()
        {
            return audioEngineRef._createEngine();
        }

        public void EnginePrepareToPlay(IntPtr engine, double sampleRate, int samplesPerBlockExpected)
        {
            audioEngineRef._enginePrepareToPlay(engine, sampleRate, samplesPerBlockExpected);
        }

        public void EngineProcessWave(IntPtr engine, float[] buffer, int numSamples, int offset)
        {
            audioEngineRef._engineProcessWave(engine, buffer, numSamples, offset);
        }

        public void DestroyEngine(IntPtr engine)
        {
            audioEngineRef._destroyEngine(engine);
        }

        public void ChangeFrequency(IntPtr engine, float newFrequency)
        {
            audioEngineRef._changeFrequency(engine, newFrequency);
        }

        public void ChangeGain(IntPtr engine, float newGain)
        {
            audioEngineRef._changeGain(engine, newGain);
        }
    }
}