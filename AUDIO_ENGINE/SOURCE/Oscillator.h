#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
using namespace juce;
using namespace std;

struct Struct_DSP_Effects {
	unique_ptr<dsp::Reverb> Reverb_Efect = make_unique<dsp::Reverb>();
	unique_ptr<dsp::WaveShaper<float>> distortion = make_unique<dsp::WaveShaper<float>>();
	unique_ptr<dsp::Gain<float>> inputGain = make_unique<dsp::Gain<float>>();
	unique_ptr<dsp::Gain<float>> outputGain = make_unique<dsp::Gain<float>>();
	//unique_ptr<dsp::Compressor<float>> Compressor_Effect = make_unique<dsp::Compressor<float>>();
};

class _Oscillator
{
	public:
		//PARAMETERS
		float gain = 0.1f;
		float frequency = 50.0f; // Frequency in Hz
		float distortionDrive = 0;

		unique_ptr<Struct_DSP_Effects> DSP_EFFECTS = make_unique<Struct_DSP_Effects>();

		//METHODS
		_Oscillator();
		~_Oscillator();

		void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain);
		void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill);
		void releaseResources();

		void changeFrequency(float newFrequency);
		void changeGain(float newGain);
		void changeDistortionDrive(float newDrive);

	private:
	
		float phase = 0;
		float phaseIncrement = 0.1f;
		double _sampleRate = 44100.0;
		int numChannels = 1;

		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_Oscillator)
};