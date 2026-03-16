#pragma once

#include <../SOURCE/WaveEngineTemplate.h>
#include <../SOURCE/DSPProcessing.h>

class _FileTrack : _IEngine
{
public:
	//METHODS
	_FileTrack(std::string filePath);
	~_FileTrack();

	void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) override;
	void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray) override;
	void getBulkAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, int startSampleIndex);
	void releaseResources() override;
	char getEngineType() override;

	void changeRepeatingMode(bool newRepeatState);
	void changeFileTimePitchCouplingMode(bool newFileTimePitchCouplingMode);
	void changeFileTempo(float newTempo);
	void changeFilePitch(float newPitch);
	void resetTime();

private:
	juce::AudioFormatManager formatManager = juce::AudioFormatManager();
	std::unique_ptr<juce::AudioFormatReader> readerSource;
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