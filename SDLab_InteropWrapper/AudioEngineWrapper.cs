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

        public IntPtr CreateEngine(string musicPath)
        {
            return audioEngineRef._createAudioFileEngine(musicPath);
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

        public void ChangeWaveShapeFunction(IntPtr engine, int functionIndex)
        {
            audioEngineRef._changeWaveShapeFunction(engine, functionIndex);
        }

        public float[] PushOscVisSamples(IntPtr engine)
        {
            return audioEngineRef._pushOscVisSamples(engine);
        }

        public void ChangeAudioFileRepeatingMode(IntPtr engine, bool newRepeatState)
        {
            audioEngineRef._changeAudioFileRepeatingMode(engine, newRepeatState);
        }

        public void ChangeAudioFileTimePitchCouplingMode(IntPtr engine, bool newTimePitchCouplingMode)
        {
            audioEngineRef._changeAudioFileTimePitchCouplingMode(engine, newTimePitchCouplingMode);
        }

        public void ChangeAudioFileTempo(IntPtr engine, float newTempo)
        {
            audioEngineRef._changeAudioFileTempo(engine, newTempo);
        }

        public void ChangeAudioFilePitch(IntPtr engine, float newPitch)
        {
            audioEngineRef._changeAudioFilePitch(engine, newPitch);
        }

        #endregion EngineMgmtLogic

        #region DSPs

        public float[] PushVisSamples(IntPtr SFXDSPProcessor, int effectTypeID)
        {
            return audioEngineRef._pushVisSamples(SFXDSPProcessor, effectTypeID);
        }

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

        #region ReverbDSP

        public void ChangeReverbRoomSize(IntPtr reverbDSPProcessor, float newRoomSize)
        {
            audioEngineRef._changeReverbRoomSize(reverbDSPProcessor, newRoomSize);
        }

        public void ChangeReverbDamping(IntPtr reverbDSPProcessor, float newDamping)
        {
            audioEngineRef._changeReverbDamping(reverbDSPProcessor, newDamping);
        }

        public void ChangeReverbWetLevel(IntPtr reverbDSPProcessor, float newWetLevel)
        {
            audioEngineRef._changeReverbWetLevel(reverbDSPProcessor, newWetLevel);
        }

        public void ChangeReverbDryLevel(IntPtr reverbDSPProcessor, float newDryLevel)
        {
            audioEngineRef._changeReverbDryLevel(reverbDSPProcessor, newDryLevel);
        }

        public void ChangeReverbWidth(IntPtr reverbDSPProcessor, float newWidth)
        {
            audioEngineRef._changeReverbWidth(reverbDSPProcessor, newWidth);
        }

        public void ChangeReverbFreezeMode(IntPtr reverbDSPProcessor, bool newFreezeMode)
        {
            audioEngineRef._changeReverbFreezeMode(reverbDSPProcessor, newFreezeMode);
        }

        #endregion ReverbDSP

        #region ChorusDSP

        public void ChangeChorusRate(IntPtr chorusDSPProcessor, float newRate)
        {
            audioEngineRef._changeChorusRate(chorusDSPProcessor, newRate);
        }

        public void ChangeChorusDepth(IntPtr chorusDSPProcessor, float newDepth)
        {
            audioEngineRef._changeChorusDepth(chorusDSPProcessor, newDepth);
        }

        public void ChangeChorusCenterDelay(IntPtr chorusDSPProcessor, float newCenterDelay)
        {
            audioEngineRef._changeChorusCenterDelay(chorusDSPProcessor, newCenterDelay);
        }

        public void ChangeChorusFeedback(IntPtr chorusDSPProcessor, float newFeedback)
        {
            audioEngineRef._changeChorusFeedback(chorusDSPProcessor, newFeedback);
        }

        public void ChangeChorusMix(IntPtr chorusDSPProcessor, float newMix)
        {
            audioEngineRef._changeChorusMix(chorusDSPProcessor, newMix);
        }

        #endregion ChorusDSP

        #endregion DSPs
    }
}