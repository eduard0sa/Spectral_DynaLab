#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <../SOURCE/WaveEngineTemplate.h>
#include <../SOURCE/DSPProcessing.h>
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
	void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) override;
	void releaseResources() override;

	void changeRepeatingMode(bool newRepeatState);

private:
	AudioFormatManager formatManager = AudioFormatManager();
	unique_ptr<AudioFormatReader> readerSource;

	juce::AudioBuffer<float> tempBuffer;
	int currentSampleIndex = 0;
	bool isRepeating = false;

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_FileTrack)
};