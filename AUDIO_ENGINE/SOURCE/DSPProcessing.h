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
	Chorus,
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
	float distortionDrive = 250;

public:
	int id;
	float *visSampleArray;

	DSPDistortionEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPDistortionEffect();

	int getEffectID();

	float* pushVisSamples() {
		return visSampleArray;
	}

	void changeFunctionToUse(float(*newFunctionToUse)(float));

	void changeDistortionDrive(float newDrive);
};

class DSPCompressorEffect : public DSPEffect
{
private:
	dsp::Compressor<float> compressorSFX;

	float compressorThreshold = -10.0f;
	float compressorRatio = 2.0f;
	float compressorAttack = 20.0f;
	float compressorRelease = 500.0f;

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

class DSPReverbEffect : public DSPEffect
{
private:
	dsp::Reverb reverbSFX;

	dsp::Reverb::Parameters reverbSFXParams = dsp::Reverb::Parameters();

	float roomSize = 0.5f; // 0 - 1
	float damping = 0.5f; // 0 - 1
	float wetLevel = 0.5f; // 0 - 1
	float dryLevel = 1.0f; // 0 - 1
	float width = 0.5f; // 0 - 1
	float freezeMode = 0.0f; // 0 - 1

public:
	int id;

	DSPReverbEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPReverbEffect();

	int getEffectID();

	void changeReverbRoomSize(float newRoomSize);
	void changeReverbDamping(float newDamping);
	void changeReverbWetLevel(float newWetLevel);
	void changeReverbDryLevel(float newDryLevel);
	void changeReverbWidth(float newWidth);
	void changeReverbFreezeMode(bool newFreezeMode);
};

class DSPChorusEffect : public DSPEffect
{
private:
	dsp::Chorus<float> chorusSFX;

	float rate = 0.8f;
	float depth = 0.4f;
	float centerDelay = 10.0f;
	float feedback = 0.0f;
	float mix = 0.5f;

public:
	int id;

	DSPChorusEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPChorusEffect();

	int getEffectID();

	void changeChorusRate(float newRate);
	void changeChorusDepth(float newDepth);
	void changeChorusCenterDelay(float newCenterDelay);
	void changeChorusFeedback(float newFeedback);
	void changeChorusMix(float newMix);
};