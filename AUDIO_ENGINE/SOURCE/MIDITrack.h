#pragma once

#include <../SOURCE/WaveEngineTemplate.h>
#include <../SOURCE/global.h>
#include <string>

class _MIDITrack : _IEngine
{
public:
	//METHODS
	_MIDITrack();
	~_MIDITrack();

	void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) override;
	void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill, bool fillVisualizationArray) override;
	void releaseResources() override;
	string getEngineType() override;

	void changeRepeatingMode(bool newRepeatState);
	void changeFileTempo(float newTempo);
	void SetMIDITemplateSamplingProvider(_IEngine* audioProvider);
	void RenderMIDIWaveform(std::vector<std::vector<struct_noteInfo>> notesPitchRatioArr, int notesCount, int maxNotesPerColumn);

private:
	std::unique_ptr<RubberBand::RubberBandStretcher> rbbStretcher;

	juce::AudioBuffer<float> MIDITrackBuffer;
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