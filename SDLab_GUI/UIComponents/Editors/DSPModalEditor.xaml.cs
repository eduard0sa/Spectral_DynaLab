using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.UIComponents.TrackUIComponents;

namespace SDLab_GUI.UIComponents.Editors
{
    /// <summary>
    /// DSP Modal Editor Window Content Page Class
    /// </summary>
    public partial class DSPModalEditor : ContentPage
    {
        private AudioEngineMGMT audioEngineMGMT;
        private JuceAudioProvider audioProviderOBJ;
        private EventHandler modalBoxCloseEvent;

        /// <summary>
        /// DSPModalEditor class constructor
        /// </summary>
        /// <param name="audioManager">Reference of the audioManager object, initialized in MainPage.xaml.cs.</param>
        /// <param name="audioProvider">Reference of the target oscillator audio provider.</param>
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
                    case Global.enumVariableDataType.TYPE_REVERB_DSP_CLASS:
                        dspChainStackLayout.Children.Add(audioProvider.DspProcessors[i].dataUnit as DSPEffectItem<ReverbDSP>);
                        break;
                    case Global.enumVariableDataType.TYPE_CHORUS_DSP_CLASS:
                        dspChainStackLayout.Children.Add(audioProvider.DspProcessors[i].dataUnit as DSPEffectItem<ChorusDSP>);
                        break;
                }
            }

            addDistortionDSPBTN.Clicked += addDistortionDSPBTNEvent;
            addCompressorDSPBTN.Clicked += addCompressorDSPBTNEvent;
            addReverbDSPBTN.Clicked += addReverbDSPBTNEvent;
            addChorusDSPBTN.Clicked += addChorusDSPBTNEvent;
            addEQDSPBTN.Clicked += addEQDSPBTNEvent;
            addFilterDSPBTN.Clicked += addFilterDSPBTNEvent;
        }

        /// <summary>
        /// This property hold the function that closes the modal box on user click.
        /// </summary>
        public EventHandler ModalBoxCloseEvent
        {
            get
            {
                return modalBoxCloseEvent;
            }
            set
            {
                modalBoxCloseBTN.Clicked -= modalBoxCloseEvent;
                modalBoxCloseEvent = value;
                modalBoxCloseBTN.Clicked += modalBoxCloseEvent;
            }
        }

        /// <summary>
        /// This method automatically unfocuses a slider when it is focused modal box open.
        /// </summary>
        /// <param name="sender">Slider sender object reference.</param>
        /// <param name="e">Event Handler Event Arguments.</param>
        private void SliderAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Slider originSlider = sender as Slider;
            originSlider.Unfocus();
        }

        /// <summary>
        /// This method appends a distortionDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDistortionDSPBTNEvent(object? sender, EventArgs e)
        {
            Global.structVariableDataTypeUnit distortionDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.DISTORTION);

            dspChainStackLayout.Children.Add(distortionDSPUnit.dataUnit as DSPEffectItem<DistortionDSP>);
        }

        /// <summary>
        /// This method appends a compressorDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCompressorDSPBTNEvent(object? sender, EventArgs e)
        {
            Global.structVariableDataTypeUnit compressorDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.COMPRESSOR);

            dspChainStackLayout.Children.Add(compressorDSPUnit.dataUnit as DSPEffectItem<CompressorDSP>);
        }

        /// <summary>
        /// This method appends a reverbDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addReverbDSPBTNEvent(object? sender, EventArgs e)
        {
            Global.structVariableDataTypeUnit reverbDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.REVERB);

            dspChainStackLayout.Children.Add(reverbDSPUnit.dataUnit as DSPEffectItem<ReverbDSP>);
        }

        /// <summary>
        /// This method appends a chorusDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addChorusDSPBTNEvent(object? sender, EventArgs e)
        {
            Global.structVariableDataTypeUnit chorusDSPUnit = audioProviderOBJ.addDSPEffect(Global.enumDSPType.CHORUS);

            dspChainStackLayout.Children.Add(chorusDSPUnit.dataUnit as DSPEffectItem<ChorusDSP>);
        }

        /// <summary>
        /// This method appends a EQDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addEQDSPBTNEvent(object? sender, EventArgs e)
        {
            /*DSPEffectItem equalizerItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.EQ);
            dspChainStackLayout.Children.Add(equalizerItem);*/
        }

        /// <summary>
        /// This method appends a filterDSPEffect to the DSP Chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilterDSPBTNEvent(object? sender, EventArgs e)
        {
            /*DSPEffectItem filterItem = new DSPEffectItem(audioManager, audioProvider, Global.enumDSPType.FILTER);
            dspChainStackLayout.Children.Add(filterItem);*/
        }
    }
}