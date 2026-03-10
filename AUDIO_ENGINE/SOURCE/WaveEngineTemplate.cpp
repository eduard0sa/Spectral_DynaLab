#include "WaveEngineTemplate.h"

using namespace juce;
using namespace std;

void _IEngine::changeGain(float newGain) {
	gain = newGain;
	outputGain->setGainLinear(gain);
}

float* _IEngine::pushOscVisSamples() {
	return visSampleArrayHEAP;
}

void _IEngine::removeDSPEffect(void* effect) {
	for (int i = 0; i < DSPEffectChainLength; i++) {
		if (DSPEffectChain[i]->getEffectID() == ((DSPEffect*)effect)->getEffectID()) {
			free(DSPEffectChain[i]);
			DSPEffectChain[i] = NULL;

			for (int j = i; j < DSPEffectChainLength; j++) {
				DSPEffectChain[j] = DSPEffectChain[j + 1];
			}

			DSPEffectChainLength--;
			break;
		}
	}
}

void _IEngine::setBlockSize(int newBlockSize) {
	spec.maximumBlockSize = newBlockSize;
}

bool _IEngine::checkExistantEffectID(int id) {
    for (int i = 0; i < DSPEffectChainLength; i++) {
        if (DSPEffectChain[i]->getEffectID() == id) {
            return true;
        }
    }
    return false;
}