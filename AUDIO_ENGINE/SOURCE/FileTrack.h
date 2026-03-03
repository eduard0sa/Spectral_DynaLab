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

class _FileTrack : _IEngine
{
public:
	//METHODS
	_FileTrack(std::string filePath);
	~_FileTrack();

	void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) override;
	void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray) override;
	void releaseResources() override;

	void changeRepeatingMode(bool newRepeatState);
	void changeFileTimePitchCouplingMode(bool newFileTimePitchCouplingMode);
	void changeFileTempo(float newTempo);
	void changeFilePitch(float newPitch);

private:
	AudioFormatManager formatManager = AudioFormatManager();
	unique_ptr<AudioFormatReader> readerSource;
	std::unique_ptr<RubberBand::RubberBandStretcher> rbbStretcher;

	juce::AudioBuffer<float> tempBuffer;
	int currentSampleIndex = 0;
	bool isRepeating = false;

	bool timePitchCouplingMode = true;

	float currentSampleContinuousPosition = 0;
	float internalTempoRatio = 1.0f;
	float setTempoRatio = 1.0f;
	float setPitchRatio = 1.0f;

	float resampleSample(int channelIndex, float sampleIndex, float _pitchRatio);

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_FileTrack)
};