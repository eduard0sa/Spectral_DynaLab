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

        #region EngineMgmtLogic

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

        #endregion EngineMgmtLogic

        #region DSPs

        public IntPtr AddDSPEffect(IntPtr engine, int effecttypeID)
        {
            return audioEngineRef._addDSPEffect(engine, effecttypeID);
        }

        public void RemoveDSPEffect(IntPtr engine, IntPtr effectDSPProcessor)
        {
            audioEngineRef._removeDSPEffect(engine, effectDSPProcessor);
        }

        #region DistortionDSP

        public void ChangeDistortionDrive(IntPtr distortionDSPProcessor, float newDrive)
        {
            audioEngineRef._changeDistortionDrive(distortionDSPProcessor, newDrive);
        }

        public void ChangeDistortionFunctionToUse(IntPtr distortionDSPProcessor, int newFunctionIndex)
        {
            audioEngineRef._changeDistortionDrive(distortionDSPProcessor, newFunctionIndex);
        }

        #endregion DistortionDSP

        #region CompressorDSP

        public void ChangeCompressorThreshold(IntPtr compressorDSPProcessor, float newThreshold)
        {
            audioEngineRef._changeCompressorThreshold(compressorDSPProcessor, newThreshold);
        }

        public void ChangeCompressorRatio(IntPtr compressorDSPProcessor, float newRatio)
        {
            audioEngineRef._changeCompressorRatio(compressorDSPProcessor, newRatio);
        }

        public void ChangeCompressorAttack(IntPtr compressorDSPProcessor, float newAttack)
        {
            audioEngineRef._changeCompressorAttack(compressorDSPProcessor, newAttack);
        }

        public void ChangeCompressorRelease(IntPtr compressorDSPProcessor, float newRelease)
        {
            audioEngineRef._changeCompressorRelease(compressorDSPProcessor, newRelease);
        }

        #endregion CompressorDSP
        #endregion DSPs
    }
}