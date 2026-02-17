#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <../SOURCE/WaveEngineTemplate.h>
#include <../SOURCE/DSPProcessing.h>
#include <stdlib.h>

using namespace juce;
using namespace std;

enum enum_OscillatorWaveShapeType {
	sine,
	square,
	triangle
};

class _Oscillator : _IEngine
{
	public:
		//PARAMETERS
		float frequency = 50.0f; // Frequency in Hz
		enum_OscillatorWaveShapeType waveShape;

		//METHODS
		_Oscillator();
		~_Oscillator();

		void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) override;
		void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) override;
		void releaseResources() override;

		void changeFrequency(float newFrequency);

		void changeWaveShapeFunction(enum_OscillatorWaveShapeType functionType);

	private:
		float phase = 0;
		float phaseIncrement = 0.1f;

		float processWaveShapeFunction(float phase);

		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_Oscillator)
};