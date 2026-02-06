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
	visSampleArray = context.getOutputBlock().getChannelPointer(0);
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

DSPReverbEffect::DSPReverbEffect() {}

void DSPReverbEffect::prepare(juce::dsp::ProcessSpec& spec) {
	reverbSFX.prepare(spec);

	reverbSFXParams.roomSize = roomSize;
	reverbSFXParams.damping = damping;
	reverbSFXParams.wetLevel = wetLevel;
	reverbSFXParams.dryLevel = dryLevel;
	reverbSFXParams.width = width;
	reverbSFXParams.freezeMode = freezeMode;

	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	reverbSFX.process(context);
}

DSPReverbEffect::~DSPReverbEffect() {}

int DSPReverbEffect::getEffectID() {
	return id;
}

void DSPReverbEffect::changeReverbRoomSize(float newRoomSize) {
	reverbSFXParams.roomSize = newRoomSize;
	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::changeReverbDamping(float newDamping) {
	reverbSFXParams.damping = newDamping;
	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::changeReverbWetLevel(float newWetLevel) {
	reverbSFXParams.wetLevel = newWetLevel;
	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::changeReverbDryLevel(float newDryLevel) {
	reverbSFXParams.dryLevel = dryLevel;
	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::changeReverbWidth(float newWidth) {
	reverbSFXParams.width = newWidth;
	reverbSFX.setParameters(reverbSFXParams);
}

void DSPReverbEffect::changeReverbFreezeMode(bool newFreezeMode) {
	reverbSFXParams.freezeMode = newFreezeMode == true ? 1.0f : 0.0f;
	reverbSFX.setParameters(reverbSFXParams);
}

#pragma endregion REVERB

#pragma region CHORUS

DSPChorusEffect::DSPChorusEffect() {
	chorusSFX = dsp::Chorus<float>();
}

void DSPChorusEffect::prepare(juce::dsp::ProcessSpec& spec) {
	chorusSFX.prepare(spec);

	chorusSFX.setRate(rate);
	chorusSFX.setDepth(depth);
	chorusSFX.setCentreDelay(centerDelay);
	chorusSFX.setFeedback(feedback);
	chorusSFX.setMix(mix);
}

void DSPChorusEffect::process(juce::dsp::ProcessContextReplacing<float> context) {
	chorusSFX.process(context);
}

DSPChorusEffect::~DSPChorusEffect() {

}

int DSPChorusEffect::getEffectID() {
	return id;
}

void DSPChorusEffect::changeChorusRate(float newRate) {
	rate = newRate;
	chorusSFX.setRate(rate);
}

void DSPChorusEffect::changeChorusDepth(float newDepth) {
	depth = newDepth;
	chorusSFX.setDepth(depth);
}

void DSPChorusEffect::changeChorusCenterDelay(float newCenterDelay) {
	centerDelay = newCenterDelay;
	chorusSFX.setCentreDelay(centerDelay);
}

void DSPChorusEffect::changeChorusFeedback(float newFeedback) {
	feedback = newFeedback;
	chorusSFX.setFeedback(feedback);
}

void DSPChorusEffect::changeChorusMix(float newMix) {
	mix = newMix;
	chorusSFX.setMix(mix);
}

#pragma endregion CHORUS