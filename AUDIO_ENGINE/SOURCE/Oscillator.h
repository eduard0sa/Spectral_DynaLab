#pragma once

#include<../JuceLibraryCode/JuceHeader.h>
#include<juce_dsp/juce_dsp.h>
#include <../SOURCE/DSPProcessing.h>
#include <stdlib.h>
using namespace juce;
using namespace std;

class _Oscillator
{
	public:
		//PARAMETERS
		float gain = 0.1f;
		float frequency = 50.0f; // Frequency in Hz

		//METHODS
		_Oscillator();
		~_Oscillator();

		void prepareToPlay(int samplesPerBlockExpected, double sampleRate, float initFrequency, float initGain);
		void getNextAudioBlock(const juce::AudioSourceChannelInfo& bufferToFill);
		void releaseResources();

		void changeFrequency(float newFrequency);
		void changeGain(float newGain);

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
		}

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