#include "DSPProcessing.h"

#pragma region GAIN

DSPGainEffect::DSPGainEffect() {
	gainSFX = dsp::Gain<float>();
}

void DSPGainEffect::prepare(juce::dsp::ProcessSpec& spec) {
	gainSFX.prepare(spec);
}

void DSPGainEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	gainSFX.process(context);
}

DSPGainEffect::~DSPGainEffect() {}

int DSPGainEffect::getEffectID() {
	return id;
}

void DSPGainEffect::setGainLinear(float newGain) {
	gainSFX.setGainLinear(newGain);
}

#pragma endregion GAIN

#pragma region DISTORTION

DSPDistortionEffect::DSPDistortionEffect() {
	inputGain = dsp::Gain<float>();
	distortionSFX = dsp::WaveShaper<float>();
}

void DSPDistortionEffect::prepare(juce::dsp::ProcessSpec& spec) {
	inputGain.prepare(spec);
	distortionSFX.prepare(spec);

	changeDistortionDrive(distortionDrive);
	changeFunctionToUse([](float sample)
		{
			return tanh(sample);
		});
}

void DSPDistortionEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	inputGain.process(context);
	distortionSFX.process(context);
}

DSPDistortionEffect::~DSPDistortionEffect() {}

int DSPDistortionEffect::getEffectID() {
	return id;
}

void DSPDistortionEffect::changeFunctionToUse(float(*newFunctionToUse)(float)) {
	distortionSFX.functionToUse = newFunctionToUse;
}

void DSPDistortionEffect::changeDistortionDrive(float newDrive) {
	distortionDrive = newDrive;
	inputGain.setGainLinear(1 + distortionDrive);
}

#pragma endregion DISTORTION

#pragma region COMPRESSOR

DSPCompressorEffect::DSPCompressorEffect() {
	compressorSFX = dsp::Compressor<float>();
}

void DSPCompressorEffect::prepare(juce::dsp::ProcessSpec& spec) {
	compressorSFX.prepare(spec);
	compressorSFX.setThreshold(compressorThreshold);
	compressorSFX.setRatio(compressorRatio);
	compressorSFX.setAttack(compressorAttack);
	compressorSFX.setRelease(compressorRelease);
}

void DSPCompressorEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	compressorSFX.process(context);
}

DSPCompressorEffect::~DSPCompressorEffect() {}

int DSPCompressorEffect::getEffectID() {
	return id;
}

void DSPCompressorEffect::changeCompressorThreshold(float newThreshold) {
	compressorThreshold = newThreshold;
	compressorSFX.setThreshold(compressorThreshold);
}

void DSPCompressorEffect::changeCompressorRatio(float newRatio) {
	compressorRatio = newRatio;
	compressorSFX.setRatio(compressorRatio);
}

void DSPCompressorEffect::changeCompressorAttack(float newAttack) {
	compressorAttack = newAttack;
	compressorSFX.setAttack(compressorAttack);
}

void DSPCompressorEffect::changeCompressorRelease(float newRelease) {
	compressorRelease = newRelease;
	compressorSFX.setRelease(compressorRelease);
}

#pragma endregion COMPRESSOR

#pragma region REVERB

DSPReverbEffect::DSPReverbEffect() {
	reverbSFX = dsp::Reverb();
}

void DSPReverbEffect::prepare(juce::dsp::ProcessSpec& spec) {
	reverbSFX.prepare(spec);
}

void DSPReverbEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	reverbSFX.process(context);
}

DSPReverbEffect::~DSPReverbEffect() {}

int DSPReverbEffect::getEffectID() {
	return id;
}

#pragma endregion REVERB