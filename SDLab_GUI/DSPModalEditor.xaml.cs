namespace SDLab_GUI;

public partial class DSPModalEditor : ContentPage
{
    private AudioEngineMGMT audioEngineMGMT;
    private JuceAudioProvider audioProviderOBJ;
    private EventHandler modalBoxCloseEvent;

    public DSPModalEditor(AudioEngineMGMT audioManager, JuceAudioProvider audioProvider)
	{
		InitializeComponent();

        // Additional initialization code can be added here
        audioEngineMGMT = audioManager;
        audioProviderOBJ = audioProvider;

        dspChainStackLayout.Children.Clear();

        for (int i = 0; i < audioProvider.DspProcessors.Count; i++)
        {
            switch (audioProvider.DspProcessors[i].dataType)
            {
                case Global.enumVariableDataType.TYPE_DISTORTION_DSP_CLASS:
                    dspChainStackLayout.Children.Add(audioProvider.DspProcessors[i].dataUnit as DSPEffectItem<DistortionDSP>);
                    break;
                case Global.enumVariableDataType.TYPE_COMPRESSOR_DSP_CLASS:
                    dspChainStackLayout.Children.Add(audioProvider.DspProcessors[i].dataUnit as DSPEffectItem<CompressorDSP>);
                    break;
            }
        }

        addDistortionDSPBTN.Clicked += addDistortionDSPBTNEvent;
        addCompressorDSPBTN.Clicked += addCompressorDSPBTNEvent;
        addReverbDSPBTN.Clicked += addReverbDSPBTNEvent;
        addEQDSPBTN.Clicked += addEQDSPBTNEvent;
        addFilterDSPBTN.Clicked += addFilterDSPBTNEvent;
    }

    public EventHandler ModalBoxCloseEvent {
        get {
            return modalBoxCloseEvent;
        }
        set {
            modalBoxCloseBTN.Clicked -= modalBoxCloseEvent;
            modalBoxCloseEvent = value;
            modalBoxCloseBTN.Clicked += modalBoxCloseEvent;
        }
    }

    private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
    {
        Slider originSlider = sender as Slider;
        originSlider.Unfocus();
    }

    private void addDistortionDSPBTNEvent(object? sender, EventArgs e)
    {
        Global.structVariableDataTypeUnit distortionDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.DISTORTION);

        dspChainStackLayout.Children.Add(distortionDSPUnit.dataUnit as DSPEffectItem<DistortionDSP>);
    }

    private void addCompressorDSPBTNEvent(object? sender, EventArgs e)
    {
        Global.structVariableDataTypeUnit compressorDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.COMPRESSOR);

        dspChainStackLayout.Children.Add(compressorDSPUnit.dataUnit as DSPEffectItem<CompressorDSP>);
    }

    private void addReverbDSPBTNEvent(object? sender, EventArgs e)
    {
        /*DSPEffectItem reverbItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.REVERB);
        dspChainStackLayout.Children.Add(reverbItem);*/
    }

    private void addEQDSPBTNEvent(object? sender, EventArgs e)
    {
        /*DSPEffectItem equalizerItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.EQ);
        dspChainStackLayout.Children.Add(equalizerItem);*/
    }

    private void addFilterDSPBTNEvent(object? sender, EventArgs e)
    {
        /*DSPEffectItem filterItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.FILTER);
        dspChainStackLayout.Children.Add(filterItem);*/
    }
}