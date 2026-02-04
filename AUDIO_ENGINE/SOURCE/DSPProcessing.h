#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>

using namespace juce;
using namespace std;

/*struct Struct_DSP_Effects {
	unique_ptr<dsp::Reverb> Reverb_Efect = make_unique<dsp::Reverb>();
	unique_ptr<dsp::WaveShaper<float>> distortion = make_unique<dsp::WaveShaper<float>>();
	unique_ptr<dsp::Gain<float>> inputGain = make_unique<dsp::Gain<float>>();
	unique_ptr<dsp::Gain<float>> outputGain = make_unique<dsp::Gain<float>>();
	//unique_ptr<dsp::Compressor<float>> Compressor_Effect = make_unique<dsp::Compressor<float>>();
};*/

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

	DSPGainEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;
	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPGainEffect();

	int getEffectID();

	void setGainLinear(float newGain);
};

class DSPDistortionEffect : public DSPEffect
{
private:
	dsp::Gain<float> inputGain;
	dsp::WaveShaper<float> distortionSFX;
	float distortionDrive = 500;

public:
	int id;

	DSPDistortionEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPDistortionEffect();

	int getEffectID();

	void changeFunctionToUse(float(*newFunctionToUse)(float));

	void changeDistortionDrive(float newDrive);
};

class DSPCompressorEffect : public DSPEffect
{
private:
	dsp::Compressor<float> compressorSFX;

	float compressorThreshold = -20.0f;
	float compressorRatio = 4.0f;
	float compressorAttack = 10.0f;
	float compressorRelease = 100.0f;

public:
	int id;

	DSPCompressorEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPCompressorEffect();

	int getEffectID();

	void changeCompressorThreshold(float newThreshold);

	void changeCompressorRatio(float newRatio);

	void changeCompressorAttack(float newAttack);

	void changeCompressorRelease(float newRelease);
};