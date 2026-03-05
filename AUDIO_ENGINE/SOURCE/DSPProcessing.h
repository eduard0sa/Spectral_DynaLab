#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <memory>

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
	juce::dsp::Gain<float> gainSFX;

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
	juce::dsp::Gain<float> inputGain;
	juce::dsp::WaveShaper<float> distortionSFX;
	float distortionDrive = 250;

public:
	int id;
	float *visSampleArrayHEAP;
	float *visSampleArraySTACK;

	DSPDistortionEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPDistortionEffect();

	int getEffectID();

	float* pushVisSamples();

	void changeFunctionToUse(float(*newFunctionToUse)(float));

	void changeDistortionDrive(float newDrive);
};

class DSPCompressorEffect : public DSPEffect
{
private:
	juce::dsp::Compressor<float> compressorSFX;

	float compressorThreshold = -10.0f;
	float compressorRatio = 2.0f;
	float compressorAttack = 20.0f;
	float compressorRelease = 500.0f;

public:
	int id;
	float* visSampleArrayHEAP;
	float* visSampleArraySTACK;

	DSPCompressorEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPCompressorEffect();

	int getEffectID();

	float* pushVisSamples();

	void changeCompressorThreshold(float newThreshold);

	void changeCompressorRatio(float newRatio);

	void changeCompressorAttack(float newAttack);

	void changeCompressorRelease(float newRelease);
};

class DSPReverbEffect : public DSPEffect
{
private:
	juce::dsp::Reverb reverbSFX;

	juce::dsp::Reverb::Parameters reverbSFXParams = juce::dsp::Reverb::Parameters();

	float roomSize = 0.5f; // 0 - 1
	float damping = 0.5f; // 0 - 1
	float wetLevel = 0.5f; // 0 - 1
	float dryLevel = 1.0f; // 0 - 1
	float width = 0.5f; // 0 - 1
	float freezeMode = 0.0f; // 0 - 1

public:
	int id;
	float* visSampleArrayHEAP;
	float* visSampleArraySTACK;

	DSPReverbEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPReverbEffect();

	int getEffectID();

	float* pushVisSamples();

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
	juce::dsp::Chorus<float> chorusSFX;

	float rate = 0.8f;
	float depth = 0.4f;
	float centerDelay = 10.0f;
	float feedback = 0.0f;
	float mix = 0.5f;

public:
	int id;
	float* visSampleArrayHEAP;
	float* visSampleArraySTACK;

	DSPChorusEffect();

	void prepare(juce::dsp::ProcessSpec& spec) override;

	void process(juce::dsp::ProcessContextReplacing<float> context) override;

	~DSPChorusEffect();

	int getEffectID();

	float* pushVisSamples();

	void changeChorusRate(float newRate);
	void changeChorusDepth(float newDepth);
	void changeChorusCenterDelay(float newCenterDelay);
	void changeChorusFeedback(float newFeedback);
	void changeChorusMix(float newMix);
};