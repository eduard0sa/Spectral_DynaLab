using SDLab_GUI.Configurations;
using SDLab_GUI.AudioSystemsLogic;
using System.Windows.Forms;

namespace SDLab_GUI.UIComponents.Editors
{
    public partial class SettingsModalEditor : ContentPage
    {
        private EditorConfigs editorConfigs;
        private bool changesSaved = true;

        public SettingsModalEditor(EditorConfigs _editorConfigs)
        {
            InitializeComponent();

            editorConfigs = _editorConfigs;

            loadOscillatorConfigs();
            loadFileTrackConfigs();
            loadMIDITrackConfigs();
            loadDSPConfigs();

            saveChangesBTN_Clicked(new object(), new EventArgs());
        }

        private void loadOscillatorConfigs()
        {
            oscillatorDefaultFrequency.Text = editorConfigs._OscillatorSettigns.DefaultFrequency.ToString();
            oscillatorDefaultFrequency.TextChanged += delegate { float a; float.TryParse(oscillatorDefaultFrequency.Text, out a); editorConfigs._OscillatorSettigns.DefaultFrequency = a; };
            oscillatorDefaultGain.Text = editorConfigs._OscillatorSettigns.DefaultVolume.ToString();
            oscillatorDefaultGain.TextChanged += delegate { float a; float.TryParse(oscillatorDefaultGain.Text, out a); editorConfigs._OscillatorSettigns.DefaultVolume = a; };
            oscillatorDefaultWaveFormat.SelectedIndex = (int)editorConfigs._OscillatorSettigns.DefaultWaveFormat;
            oscillatorDefaultWaveFormat.SelectedIndexChanged += delegate { editorConfigs._OscillatorSettigns.DefaultWaveFormat = (Global.enumWaveShapeType)oscillatorDefaultWaveFormat.SelectedIndex; };
        }
        
        private void loadFileTrackConfigs()
        {
            fileTrackDefaultGain.Text = editorConfigs._FileTrackSettings.DefaultGain.ToString();
            fileTrackDefaultGain.TextChanged += delegate { float a; float.TryParse(fileTrackDefaultGain.Text, out a); editorConfigs._FileTrackSettings.DefaultGain = a; };
            fileTrackDefaultRepeatMode.IsToggled = editorConfigs._FileTrackSettings.DefaultRepeatMode;
            fileTrackDefaultRepeatMode.Toggled += delegate { editorConfigs._FileTrackSettings.DefaultRepeatMode = fileTrackDefaultRepeatMode.IsToggled; };
            fileTrackDefaultTempo.Text = editorConfigs._FileTrackSettings.DefaultTempo.ToString();
            fileTrackDefaultTempo.TextChanged += delegate { float a; float.TryParse(fileTrackDefaultTempo.Text, out a); editorConfigs._FileTrackSettings.DefaultTempo = a; };
            fileTrackDefaultPitch.Text = editorConfigs._FileTrackSettings.DefaultPitch.ToString();
            fileTrackDefaultPitch.TextChanged += delegate { float a; float.TryParse(fileTrackDefaultPitch.Text, out a); editorConfigs._FileTrackSettings.DefaultPitch = a; };
            fileTrackDefaultTimePitchCouplingMode.IsToggled = editorConfigs._FileTrackSettings.DefaultTimePitchCouplingMode;
            fileTrackDefaultTimePitchCouplingMode.Toggled += delegate { editorConfigs._FileTrackSettings.DefaultTimePitchCouplingMode = fileTrackDefaultTimePitchCouplingMode.IsToggled; };
        }

        private void loadMIDITrackConfigs()
        {
            MIDITrackDefaultGain.Text = editorConfigs._MidiTrackSettings.DefaultGain.ToString();
            MIDITrackDefaultGain.TextChanged += delegate { float a; float.TryParse(MIDITrackDefaultGain.Text, out a); editorConfigs._MidiTrackSettings.DefaultGain = a; };
            MIDITrackDefaultRepeatMode.IsToggled = editorConfigs._MidiTrackSettings.DefaultRepeatMode;
            MIDITrackDefaultRepeatMode.Toggled += delegate { editorConfigs._MidiTrackSettings.DefaultRepeatMode = MIDITrackDefaultRepeatMode.IsToggled; };
            MIDITrackDefaultOscillatorBaseFrequency.Text = editorConfigs._MidiTrackSettings.DefaultOscillatorVaseFrequency.ToString();
            MIDITrackDefaultOscillatorBaseFrequency.TextChanged += delegate { float a; float.TryParse(MIDITrackDefaultOscillatorBaseFrequency.Text, out a); editorConfigs._MidiTrackSettings.DefaultOscillatorVaseFrequency = a; };
            MIDITrackDefaultOscillatorGain.Text = editorConfigs._MidiTrackSettings.DefaultOscillatorBaseGain.ToString();
            MIDITrackDefaultOscillatorGain.TextChanged += delegate { float a; float.TryParse(MIDITrackDefaultOscillatorGain.Text, out a); editorConfigs._MidiTrackSettings.DefaultOscillatorBaseGain = a; };
            MIDITrackDefaultOscillatorWaveFormat.SelectedIndex = (int)editorConfigs._MidiTrackSettings.DefaultOscillatorWaveFormat;
            MIDITrackDefaultOscillatorWaveFormat.SelectedIndexChanged += delegate { editorConfigs._MidiTrackSettings.DefaultOscillatorWaveFormat = (Global.enumWaveShapeType)MIDITrackDefaultOscillatorWaveFormat.SelectedIndex; };
            MIDITrackDefaultFileTrackTempo.Text = editorConfigs._MidiTrackSettings.DefaultTempo.ToString();
            MIDITrackDefaultFileTrackTempo.TextChanged += delegate { float a; float.TryParse(MIDITrackDefaultFileTrackTempo.Text, out a); editorConfigs._MidiTrackSettings.DefaultTempo = a; };
            MIDITrackDefaultFileTrackPitch.Text = editorConfigs._MidiTrackSettings.DefaultPitch.ToString();
            MIDITrackDefaultFileTrackPitch.TextChanged += delegate { float a; float.TryParse(MIDITrackDefaultFileTrackPitch.Text, out a); editorConfigs._MidiTrackSettings.DefaultPitch = a; };
            MIDITrackDefaultFileTrackTimePitchCouplingMode.IsToggled = editorConfigs._MidiTrackSettings.DefaultTimePitchCouplingMode;
            MIDITrackDefaultFileTrackTimePitchCouplingMode.Toggled += delegate { editorConfigs._MidiTrackSettings.DefaultTimePitchCouplingMode = MIDITrackDefaultFileTrackTimePitchCouplingMode.IsToggled; };
        }

        private void loadDSPConfigs()
        {
            DefaultDistortionDrive.Text = editorConfigs._DspSettings.DistortionSettings.DefaultDistortionDrive.ToString();
            DefaultDistortionDrive.TextChanged += delegate { float a; float.TryParse(DefaultDistortionDrive.Text, out a); editorConfigs._DspSettings.DistortionSettings.DefaultDistortionDrive = a; };
            DefaultDistortionType.SelectedIndex = (int)editorConfigs._DspSettings.DistortionSettings.DefaultDistortionType1;
            DefaultDistortionType.SelectedIndexChanged += delegate { editorConfigs._DspSettings.DistortionSettings.DefaultDistortionType1 = (DistortionDSP.enum_distortionType)DefaultDistortionType.SelectedIndex; };
            DefaultCompressorThreshold.Text = editorConfigs._DspSettings.CompressorSettings.DefaultThreshold.ToString();
            DefaultCompressorThreshold.TextChanged += delegate { float a; float.TryParse(DefaultCompressorThreshold.Text, out a); editorConfigs._DspSettings.CompressorSettings.DefaultThreshold = a; };
            DefaultCompressorRatio.Text = editorConfigs._DspSettings.CompressorSettings.DefaultRatio.ToString();
            DefaultCompressorRatio.TextChanged += delegate { float a; float.TryParse(DefaultCompressorRatio.Text, out a); editorConfigs._DspSettings.CompressorSettings.DefaultRatio = a; };
            DefaultCompressorAttack.Text = editorConfigs._DspSettings.CompressorSettings.DefaultAttack.ToString();
            DefaultCompressorAttack.TextChanged += delegate { float a; float.TryParse(DefaultCompressorAttack.Text, out a); editorConfigs._DspSettings.CompressorSettings.DefaultAttack = a; };
            DefaultCompressorRelease.Text = editorConfigs._DspSettings.CompressorSettings.DefaultRelease.ToString();
            DefaultCompressorRelease.TextChanged += delegate { float a; float.TryParse(DefaultCompressorRelease.Text, out a); editorConfigs._DspSettings.CompressorSettings.DefaultRelease = a; };
            DefaultReverbRoomSize.Text = editorConfigs._DspSettings.ReverbSettings.DefaultRoomSize.ToString();
            DefaultReverbRoomSize.TextChanged += delegate { float a; float.TryParse(DefaultReverbRoomSize.Text, out a); editorConfigs._DspSettings.ReverbSettings.DefaultRoomSize = a; };
            DefaultReverbDamping.Text = editorConfigs._DspSettings.ReverbSettings.DefaultDamping.ToString();
            DefaultReverbDamping.TextChanged += delegate { float a; float.TryParse(DefaultReverbDamping.Text, out a); editorConfigs._DspSettings.ReverbSettings.DefaultDamping = a; };
            DefaultReverbWetLevel.Text = editorConfigs._DspSettings.ReverbSettings.DefaultWetLevel.ToString();
            DefaultReverbWetLevel.TextChanged += delegate { float a; float.TryParse(DefaultReverbWetLevel.Text, out a); editorConfigs._DspSettings.ReverbSettings.DefaultWetLevel = a; };
            DefaultReverbDryLevel.Text = editorConfigs._DspSettings.ReverbSettings.DefaultDryLevel.ToString();
            DefaultReverbDryLevel.TextChanged += delegate { float a; float.TryParse(DefaultReverbDryLevel.Text, out a); editorConfigs._DspSettings.ReverbSettings.DefaultDryLevel = a; };
            DefaultReverbWidth.Text = editorConfigs._DspSettings.ReverbSettings.DefaultWidth.ToString();
            DefaultReverbWidth.TextChanged += delegate { float a; float.TryParse(DefaultReverbWidth.Text, out a); editorConfigs._DspSettings.ReverbSettings.DefaultWidth = a; };
            DefaultReverbFreezeMode.IsToggled = editorConfigs._DspSettings.ReverbSettings.DefaultFreezeMode;
            DefaultReverbFreezeMode.Toggled += delegate { editorConfigs._DspSettings.ReverbSettings.DefaultFreezeMode = DefaultReverbFreezeMode.IsToggled; };
            DefaultChorusRate.Text = editorConfigs._DspSettings.ChorusSettings.DefaultRate.ToString();
            DefaultChorusRate.TextChanged += delegate { float a; float.TryParse(DefaultChorusRate.Text, out a); editorConfigs._DspSettings.ChorusSettings.DefaultRate = a; };
            DefaultChorusDepth.Text = editorConfigs._DspSettings.ChorusSettings.DefaultDepth.ToString();
            DefaultChorusDepth.TextChanged += delegate { float a; float.TryParse(DefaultChorusDepth.Text, out a); editorConfigs._DspSettings.ChorusSettings.DefaultDepth = a; };
            DefaultChorusDelay.Text = editorConfigs._DspSettings.ChorusSettings.DefaultDelay.ToString();
            DefaultChorusDelay.TextChanged += delegate { float a; float.TryParse(DefaultChorusDelay.Text, out a); editorConfigs._DspSettings.ChorusSettings.DefaultDelay = a; };
            DefaultChorusFeedback.Text = editorConfigs._DspSettings.ChorusSettings.DefaultFeedback.ToString();
            DefaultChorusFeedback.TextChanged += delegate { float a; float.TryParse(DefaultChorusFeedback.Text, out a); editorConfigs._DspSettings.ChorusSettings.DefaultFeedback = a; };
            DefaultChorusMix.Text = editorConfigs._DspSettings.ChorusSettings.DefaultMix.ToString();
            DefaultChorusMix.TextChanged += delegate { float a; float.TryParse(DefaultChorusMix.Text, out a); editorConfigs._DspSettings.ChorusSettings.DefaultMix = a; };
        }

        private void textPropertyChangedEvent(object? sender, TextChangedEventArgs e)
        {
            changesSaved = false;
        }

        private void pickerPropertyChangedEvent(object? sender, EventArgs e)
        {
            changesSaved = false;
        }

        private void switchPropertyChangedEvent(object? sender, ToggledEventArgs e)
        {
            changesSaved = false;
        }

        private void saveChangesBTN_Clicked(object sender, EventArgs e)
        {
            editorConfigs.updateGlobalConfigsXML();
            changesSaved = true;
        }

        private void CloseModalBTN_Clicked(object sender, EventArgs e)
        {
            if(changesSaved == false)
            {
                MessageBox.Show("Salva as alterações antes de saíres!", "Atenção!");
            }
            else
            {
                Navigation.PopModalAsync();
            }
        }
    }
}