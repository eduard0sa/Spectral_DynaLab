#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <../SOURCE/DSPProcessing.h>
#include <stdlib.h>

using namespace juce;
using namespace std;

enum enum_engineTypes {
	oscillator,
	fileTrack
};

class _IEngine abstract {
public:
	//PARAMETERS
	float gain = 0.1f;

	//METHODS
	virtual void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain) abstract;
	virtual void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill) abstract;
	virtual void releaseResources() abstract;

	void changeGain(float newGain);

	virtual float* pushOscVisSamples();

	template<typename T>
	DSPEffect* addDSPEffect() {
		T* compressionEffectHEAP = (T*)malloc(sizeof(T));
		T* compressionEffectSTACK = new (compressionEffectHEAP) T(); //Stack reference of the memory allocated at the last row, used to call the DSPDistortionEffect's class constructor.

		Random randomizer = Random();
		int distortionEffectID;

		do {
			distortionEffectID = randomizer.nextInt(200);
		} while (checkExistantEffectID(distortionEffectID));

		compressionEffectHEAP->prepare(spec);
		compressionEffectHEAP->id = distortionEffectID;

		DSPEffectChain[DSPEffectChainLength] = compressionEffectHEAP;
		DSPEffectChainLength++;

		return compressionEffectHEAP;
	};

	void removeDSPEffect(void* effect);

	void setBlockSize(int newBlockSize) {
		spec.maximumBlockSize = newBlockSize;
	}

protected:
	juce::dsp::ProcessSpec spec = juce::dsp::ProcessSpec();

	unique_ptr<dsp::Gain<float>> outputGain = make_unique<dsp::Gain<float>>();

	double _sampleRate = 44100.0;
	int numChannels = 1;

	float* visSampleArrayHEAP;
	float* visSampleArraySTACK;

	int DSPEffectChainLength = 0;
	DSPEffect* DSPEffectChain[100];

	bool checkExistantEffectID(int id);
};