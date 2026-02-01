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

enum enum_EffectType {
	Distortion,
	Compressor,
	Reverb,
	EQ,
	Filter,
	Gain
};

struct DSPEffect {
	virtual ~DSPEffect() = default;
	virtual void prepare(juce::dsp::ProcessSpec&) = 0;
	virtual void process(juce::dsp::ProcessContextReplacing<float>) = 0;
};

struct DSPGainEffect : DSPEffect {
	unique_ptr<dsp::Gain<float>> effectPtr;

	void prepare(juce::dsp::ProcessSpec& spec) override {
		effectPtr->prepare(spec);
	}
	void process(juce::dsp::ProcessContextReplacing<float> context) override {
		effectPtr->process(context);
	}
};

struct DSPDistortionEffect : DSPEffect {
	unique_ptr<dsp::WaveShaper<float>> effectPtr;

	void prepare(juce::dsp::ProcessSpec& spec) override {
		effectPtr->prepare(spec);
	}
	void process(juce::dsp::ProcessContextReplacing<float> context) override {
		effectPtr->process(context);
	}
};

class _Oscillator
{
	public:
		//PARAMETERS
		float gain = 0.1f;
		float frequency = 50.0f; // Frequency in Hz
		float distortionDrive = 500;

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

		void addDistortionDSPEffect();

	private:
		juce::dsp::ProcessSpec spec;

		float phase = 0;
		float phaseIncrement = 0.1f;
		double _sampleRate = 44100.0;
		int numChannels = 1;

		int DSPEffectChainLength = 0;
		unique_ptr<DSPEffect> DSPEffectChain[100];
		dsp::Gain<float> inputGain = dsp::Gain<float>();
		unique_ptr<dsp::Gain<float>> outputGain = make_unique<dsp::Gain<float>>();

		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_Oscillator)
};