#include "MIDITrack.h"

_MIDITrack::_MIDITrack() {
    //formatManager.registerBasicFormats();

    visSampleArrayHEAP = (float*)malloc(sizeof(float[512]));
    visSampleArraySTACK = new (visSampleArrayHEAP) float[512]();
}

_MIDITrack::~_MIDITrack() {
    int count = 0;

    while (count < DSPEffectChainLength) {
        removeDSPEffect(DSPEffectChain[count]);
        count++;
    }
}

#pragma region Juce_Overriden_methods

void _MIDITrack::prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain)
{
    _sampleRate = sampleRate;

    spec.sampleRate = sampleRate;
    spec.maximumBlockSize = (juce::uint32)samplesPerBlockExpected;
    spec.numChannels = this->numChannels;

    outputGain->prepare(spec);
    outputGain->setGainLinear(gain);
    outputGain->setRampDurationSeconds(0.02); // super important

    int options =
        RubberBand::RubberBandStretcher::OptionProcessRealTime |
        RubberBand::RubberBandStretcher::OptionThreadingNever |
        RubberBand::RubberBandStretcher::OptionPitchHighQuality;

    rbbStretcher = std::make_unique<RubberBand::RubberBandStretcher>(spec.sampleRate, numChannels, options);
    changeFileTempo(setTempoRatio);
}

void _MIDITrack::getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray = true) {
    bufferToFill.clearActiveBufferRegion();

    if (currentSampleIndex < MIDITrackBuffer.getNumSamples()) {
        bufferToFill.clearActiveBufferRegion();
        for (int channel = 0; channel < bufferToFill.buffer->getNumChannels(); ++channel)
        {
            float* buffer = bufferToFill.buffer->getWritePointer(channel, bufferToFill.startSample);

            for (int sample = 0; sample < min(bufferToFill.numSamples, MIDITrackBuffer.getNumSamples() - currentSampleIndex); ++sample)
            {
                buffer[sample] = MIDITrackBuffer.getSample(channel, currentSampleIndex + sample);
            }
        }

        currentSampleIndex += bufferToFill.numSamples;

        juce::dsp::AudioBlock<float> audioBlock(*bufferToFill.buffer);
        juce::dsp::ProcessContextReplacing<float> context(audioBlock);

        outputGain->process(context);
    }
    else if (isRepeating == true) {
        currentSampleIndex = 0;
        currentSampleContinuousPosition = 0;
    }
}

void _MIDITrack::releaseResources()
{
    // Release any resources if needed
}

#pragma endregion

#pragma region CustomMethods

void _MIDITrack::changeRepeatingMode(bool newRepeatState) {
    isRepeating = newRepeatState;
}

void _MIDITrack::changeFileTempo(float newTempo) {
    setTempoRatio = newTempo;
    rbbStretcher->setTimeRatio(1 / setTempoRatio);
}

void _MIDITrack::SetMIDITemplateSamplingProvider(_IEngine* audioProvider) {
    templateSamplingAudioProvider = audioProvider;
}

void _MIDITrack::RenderMIDIWaveform(float** notesPitchRatioArr, int count) {
    MIDITrackBuffer.clear();

    samplesPerNoteUnit = (100 * spec.sampleRate) / 1000;

    templateSamplingAudioProvider->setBlockSize(samplesPerNoteUnit);

    MIDITrackBuffer.setSize(1, samplesPerNoteUnit * count);
    noteUnitPlanarBuffer.setSize(1, samplesPerNoteUnit);

	bufferToFill = juce::AudioSourceChannelInfo(&noteUnitPlanarBuffer, 0, samplesPerNoteUnit);
    float* buffer = MIDITrackBuffer.getWritePointer(0, 0);

    for (int i = 0; i < count; i++) {
        bufferToFill.buffer->clear();
        templateSamplingAudioProvider->getNextAudioBlock(bufferToFill, false);

		vector<juce::AudioBuffer<float>> simultaneousNotesBufferToFill;

        for (int k = 0; k < 6 * 7 + 1; k++) {
            if (notesPitchRatioArr[k][i] > 0) {
                juce::AudioBuffer<float> currentNotePlanarBuffer = juce::AudioBuffer<float>(1, samplesPerNoteUnit);
                float* currentNoteFloatBuffer = currentNotePlanarBuffer.getWritePointer(0, 0);

                for (int j = 0; j < samplesPerNoteUnit; j++) {
					currentNoteFloatBuffer[j] = (float)bufferToFill.buffer->getSample(0, j);
                }

                juce::AudioSourceChannelInfo currentNoteBufferToFill = juce::AudioSourceChannelInfo(&currentNotePlanarBuffer, 0, samplesPerNoteUnit);
                processFrequencyChange(currentNoteBufferToFill, notesPitchRatioArr[k][i]);

				simultaneousNotesBufferToFill.push_back(currentNotePlanarBuffer);
            }
        }

        for (int k = 0; k < simultaneousNotesBufferToFill.size(); k++) {
            for (int j = 0; j < samplesPerNoteUnit; j++) {
                buffer[samplesPerNoteUnit * i + j] += simultaneousNotesBufferToFill[k].getSample(0, j);
            }
        }
    }
}

void _MIDITrack::processFrequencyChange(const juce::AudioSourceChannelInfo& bufferToFill, float pitchRatio) {
	rbbStretcher->setPitchScale(pitchRatio);

    const int numChannels = bufferToFill.buffer->getNumChannels();
    const int requestedSamples = bufferToFill.numSamples;

    int availableInput = requestedSamples;

    std::vector<const float*> input(numChannels);

    while (rbbStretcher->available() < requestedSamples)
    {
        if (currentSampleIndex >= bufferToFill.buffer->getNumSamples())
            break;

        int blockToFeed = juce::jmin(
            512,
            bufferToFill.buffer->getNumSamples() - currentSampleIndex
        );

        for (int ch = 0; ch < numChannels; ++ch)
            input[ch] = bufferToFill.buffer->getReadPointer(ch, currentSampleIndex);

        rbbStretcher->process(input.data(), blockToFeed, false);

        currentSampleIndex += blockToFeed;
    }

    int availableOutput = rbbStretcher->available();

    if (availableOutput > 0)
    {
        int samplesToRetrieve = min(requestedSamples, availableOutput);

        std::vector<float*> output(numChannels);

        for (int ch = 0; ch < numChannels; ++ch)
            output[ch] = bufferToFill.buffer->getWritePointer(ch, bufferToFill.startSample);

        rbbStretcher->retrieve(output.data(), samplesToRetrieve);
    }
}

#pragma endregion