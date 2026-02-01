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

        addDistortionDSPBTN.Clicked += (s, e) =>
        {
            Global.structVariableDataTypeUnit distortionDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.DISTORTION);

            dspChainStackLayout.Children.Add(distortionDSPUnit.dataUnit as DSPEffectItem<DistortionDSP>);
        };
        addCompressorDSPBTN.Clicked += (s, e) =>
        {
            /*DSPEffectItem compressorItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.COMPRESSOR);
            dspChainStackLayout.Children.Add(compressorItem);*/
        };
        addReverbDSPBTN.Clicked += (s, e) =>
        {
            /*DSPEffectItem reverbItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.REVERB);
            dspChainStackLayout.Children.Add(reverbItem);*/
        };
        addEQDSPBTN.Clicked += (s, e) =>
        {
            /*DSPEffectItem equalizerItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.EQ);
            dspChainStackLayout.Children.Add(equalizerItem);*/
        };
        addFilterDSPBTN.Clicked += (s, e) =>
        {
            /*DSPEffectItem filterItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.FILTER);
            dspChainStackLayout.Children.Add(filterItem);*/
        };
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
}