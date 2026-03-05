#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <../SOURCE/WaveEngineTemplate.h>
#include <../SOURCE/DSPProcessing.h>
#include <RubberBandStretcher.h>
#include <string>
#include <iostream>
#include <stdlib.h>

using namespace juce;
using namespace std;

class _MIDITrack : _IEngine
{
public:
	//METHODS
	_MIDITrack();
	~_MIDITrack();

	void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) override;
	void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray) override;
	void releaseResources() override;

	void changeRepeatingMode(bool newRepeatState);
	void changeFileTempo(float newTempo);
	void SetMIDITemplateSamplingProvider(_IEngine* audioProvider);
	void RenderMIDIWaveform(float** notesPitchRatioArr, int count);

private:
	std::unique_ptr<RubberBand::RubberBandStretcher> rbbStretcher;

	juce::AudioBuffer<float> MIDITrackBuffer;
	juce::AudioBuffer<float> noteUnitPlanarBuffer;
	juce::AudioSourceChannelInfo bufferToFill;
	int samplesPerNoteUnit;

	int currentSampleIndex = 0;
	bool isRepeating = false;

	float currentSampleContinuousPosition = 0;
	float internalTempoRatio = 1.0f;
	float setTempoRatio = 1.0f;
	_IEngine* templateSamplingAudioProvider;

	void processFrequencyChange(const juce::AudioSourceChannelInfo& bufferToFill, float pitchRatio);

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_MIDITrack)
};