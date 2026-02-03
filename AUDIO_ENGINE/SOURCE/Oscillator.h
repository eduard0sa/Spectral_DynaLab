#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <stdlib.h>
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

enum enum_DistortionFunctionType {
	Softclip,
	HardClip,
	FoldBack
};

class DSPEffect
{
	public:
		virtual int getEffectID() = 0;
		virtual ~DSPEffect() = default;
		virtual void prepare(juce::dsp::ProcessSpec&) = 0;
		virtual void process(juce::dsp::ProcessContextReplacing<float>) = 0;
};

class DSPGainEffect : public DSPEffect
{
	private:
		dsp::Gain<float> gainSFX;

	public:
		int id;

		DSPGainEffect() {
			gainSFX = dsp::Gain<float>();
		}

		void prepare(juce::dsp::ProcessSpec& spec) override {
			gainSFX.prepare(spec);
		}
		void process(juce::dsp::ProcessContextReplacing<float> context) override {
			gainSFX.process(context);
		}

		~DSPGainEffect() {}

		int getEffectID() {
			return id;
		}

		void setGainLinear(float newGain) {
			gainSFX.setGainLinear(newGain);
		}
};

class DSPDistortionEffect : public DSPEffect
{
	private:
		dsp::Gain<float> inputGain;
		dsp::WaveShaper<float> distortionSFX;
		float distortionDrive = 500;

	public:
		int id;

		DSPDistortionEffect() {
			inputGain = dsp::Gain<float>();
			distortionSFX = dsp::WaveShaper<float>();
		}

		void prepare(juce::dsp::ProcessSpec& spec) override {
			inputGain.prepare(spec);
			distortionSFX.prepare(spec);

			changeDistortionDrive(distortionDrive);
			changeFunctionToUse([](float sample)
			{
				return tanh(sample);
			});
		}
		void process(juce::dsp::ProcessContextReplacing<float> context) override {
			inputGain.process(context);
			distortionSFX.process(context);
		}

		~DSPDistortionEffect() {}

		int getEffectID() {
			return id;
		}

		void changeFunctionToUse(float(*newFunctionToUse)(float)) {
			distortionSFX.functionToUse = newFunctionToUse;
		}

		void changeDistortionDrive(float newDrive) {
			distortionDrive = newDrive;
			inputGain.setGainLinear(1 + distortionDrive);
		}
};

class _Oscillator
{
	public:
		//PARAMETERS
		float gain = 0.1f;
		float frequency = 50.0f; // Frequency in Hz

		unique_ptr<Struct_DSP_Effects> DSP_EFFECTS = make_unique<Struct_DSP_Effects>();

		//METHODS
		_Oscillator();
		~_Oscillator();

		void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain);
		void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill);
		void releaseResources();

		void changeFrequency(float newFrequency);
		void changeGain(float newGain);

		DSPEffect* addDistortionDSPEffect();
		void removeDSPEffect(void* effect);

	private:
		juce::dsp::ProcessSpec spec = juce::dsp::ProcessSpec();

		float phase = 0;
		float phaseIncrement = 0.1f;
		double _sampleRate = 44100.0;
		int numChannels = 1;

		unique_ptr<dsp::Gain<float>> outputGain = make_unique<dsp::Gain<float>>();

		int DSPEffectChainLength = 0;
		DSPEffect* DSPEffectChain[100];

		bool checkExistantEffectID(int id);

		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(_Oscillator)
};