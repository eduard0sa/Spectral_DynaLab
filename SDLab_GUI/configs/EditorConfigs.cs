using System.Xml;
using SDLab_GUI.AudioSystemsLogic;
using static SDLab_GUI.Global;

namespace SDLab_GUI.Configurations
{
    public class EditorConfigs : IConfigEntity
    {
        private GeneralSettings generalSettings;
        private OscillatorSettings oscillatorSettigns;
        private FileTrackSettings fileTrackSettings;
        private MIDITrackSettings midiTrackSettings;
        private DSPSettings dspSettings;

        private string configsFilePath;

        public GeneralSettings _GeneralSettings { get => generalSettings; set => generalSettings = value; }
        public OscillatorSettings _OscillatorSettigns { get => oscillatorSettigns; set => oscillatorSettigns = value; }
        public FileTrackSettings _FileTrackSettings { get => fileTrackSettings; set => fileTrackSettings = value; }
        public MIDITrackSettings _MidiTrackSettings { get => midiTrackSettings; set => midiTrackSettings = value; }
        public DSPSettings _DspSettings { get => dspSettings; private set => dspSettings = value; }

        public EditorConfigs(string _configsFilePath)
        {
            configsFilePath = _configsFilePath;
        }

        public void loadConfigsFromFile()
        {
            XmlDocument editorConfigs = new XmlDocument();

            editorConfigs.Load(configsFilePath);

            GeneralSettings _generalSettings = new GeneralSettings();
            OscillatorSettings _oscillatorSettigns = new OscillatorSettings();
            FileTrackSettings _fileTrackSettings = new FileTrackSettings();
            MIDITrackSettings _midiTrackSettings = new MIDITrackSettings();
            DSPSettings _dspSettings = new DSPSettings();

            if (editorConfigs.DocumentElement.ChildNodes.Count == 5)
            {
                _generalSettings.DefaultGeneralVolume = float.Parse(editorConfigs.DocumentElement.ChildNodes[0].ChildNodes[0].InnerXml);

                _oscillatorSettigns.DefaultFrequency = float.Parse(editorConfigs.DocumentElement.ChildNodes[1].ChildNodes[0].InnerXml);
                _oscillatorSettigns.DefaultVolume = float.Parse(editorConfigs.DocumentElement.ChildNodes[1].ChildNodes[1].InnerXml);
                _oscillatorSettigns.DefaultWaveFormat = (Global.enumWaveShapeType)Enum.Parse(typeof(Global.enumWaveShapeType), editorConfigs.DocumentElement.ChildNodes[1].ChildNodes[2].InnerXml);

                _fileTrackSettings.DefaultGain = float.Parse(editorConfigs.DocumentElement.ChildNodes[2].ChildNodes[0].InnerXml);
                _fileTrackSettings.DefaultRepeatMode = bool.Parse(editorConfigs.DocumentElement.ChildNodes[2].ChildNodes[1].InnerXml);
                _fileTrackSettings.DefaultTempo = float.Parse(editorConfigs.DocumentElement.ChildNodes[2].ChildNodes[2].InnerXml);
                _fileTrackSettings.DefaultPitch = float.Parse(editorConfigs.DocumentElement.ChildNodes[2].ChildNodes[3].InnerXml);
                _fileTrackSettings.DefaultTimePitchCouplingMode = bool.Parse(editorConfigs.DocumentElement.ChildNodes[2].ChildNodes[4].InnerXml);

                _midiTrackSettings.DefaultGain = float.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[0].InnerXml);
                _midiTrackSettings.DefaultRepeatMode = bool.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[1].InnerXml);
                _midiTrackSettings.DefaultOscillatorVaseFrequency = float.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[2].InnerXml);
                _midiTrackSettings.DefaultOscillatorBaseGain = float.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[3].InnerXml);
                _midiTrackSettings.DefaultOscillatorWaveFormat = (Global.enumWaveShapeType)Enum.Parse(typeof(Global.enumWaveShapeType), editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[4].InnerXml);
                _midiTrackSettings.DefaultTempo = float.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[5].InnerXml);
                _midiTrackSettings.DefaultPitch = float.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[6].InnerXml);
                _midiTrackSettings.DefaultTimePitchCouplingMode = bool.Parse(editorConfigs.DocumentElement.ChildNodes[3].ChildNodes[7].InnerXml);

                _dspSettings.DistortionSettings.DefaultDistortionDrive = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[0].ChildNodes[0].InnerXml);
                _dspSettings.DistortionSettings.DefaultDistortionType1 = (DistortionDSP.enum_distortionType)Enum.Parse(typeof(DistortionDSP.enum_distortionType), editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[0].ChildNodes[1].InnerXml);
                
                _dspSettings.CompressorSettings.DefaultThreshold = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[1].ChildNodes[0].InnerXml);
                _dspSettings.CompressorSettings.DefaultRatio = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[1].ChildNodes[1].InnerXml);
                _dspSettings.CompressorSettings.DefaultAttack = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[1].ChildNodes[2].InnerXml);
                _dspSettings.CompressorSettings.DefaultRelease = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[1].ChildNodes[3].InnerXml);
                
                _dspSettings.ReverbSettings.DefaultRoomSize = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[0].InnerXml);
                _dspSettings.ReverbSettings.DefaultDamping = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[1].InnerXml);
                _dspSettings.ReverbSettings.DefaultWetLevel = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[2].InnerXml);
                _dspSettings.ReverbSettings.DefaultDryLevel = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[3].InnerXml);
                _dspSettings.ReverbSettings.DefaultWidth = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[4].InnerXml);
                _dspSettings.ReverbSettings.DefaultFreezeMode = bool.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[2].ChildNodes[5].InnerXml);
                
                _dspSettings.ChorusSettings.DefaultRate = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[3].ChildNodes[0].InnerXml);
                _dspSettings.ChorusSettings.DefaultDepth = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[3].ChildNodes[1].InnerXml);
                _dspSettings.ChorusSettings.DefaultDelay = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[3].ChildNodes[2].InnerXml);
                _dspSettings.ChorusSettings.DefaultFeedback = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[3].ChildNodes[3].InnerXml);
                _dspSettings.ChorusSettings.DefaultMix = float.Parse(editorConfigs.DocumentElement.ChildNodes[4].ChildNodes[3].ChildNodes[4].InnerXml);
            }

            _GeneralSettings = _generalSettings;
            _OscillatorSettigns = _oscillatorSettigns;
            _GeneralSettings = _generalSettings;
            _FileTrackSettings = _fileTrackSettings;
            _MidiTrackSettings = _midiTrackSettings;
            _DspSettings = _dspSettings;
        }

        public void updateGlobalConfigsXML()
        {
            XmlDocument globalConfigs = new XmlDocument();

            XmlElement root = globalConfigs.CreateElement("EDITOR_CONFIGS");
            globalConfigs.AppendChild(root);

            XmlElement generalSettings = globalConfigs.CreateElement("GENERAL_SETTINGS");
            XmlElement defaultGeneralVolume = globalConfigs.CreateElement("DefaultGeneralVolume");
            defaultGeneralVolume.InnerXml = _GeneralSettings.DefaultGeneralVolume.ToString();
            generalSettings.AppendChild(defaultGeneralVolume);

            XmlElement oscillatorSettings = globalConfigs.CreateElement("OSCILLATOR_SETTINGS");
            XmlElement defaultOscillatorFrequency = globalConfigs.CreateElement("DefaultFrequency");
            XmlElement defaultOscillatorVolume = globalConfigs.CreateElement("DefaultGain");
            XmlElement defaultOscillatorWaveFormat = globalConfigs.CreateElement("DefaultWaveFormat");
            defaultOscillatorFrequency.InnerXml = _OscillatorSettigns.DefaultFrequency.ToString();
            defaultOscillatorVolume.InnerXml = _OscillatorSettigns.DefaultVolume.ToString();
            defaultOscillatorWaveFormat.InnerXml = _OscillatorSettigns.DefaultWaveFormat.ToString();
            oscillatorSettings.AppendChild(defaultOscillatorFrequency);
            oscillatorSettings.AppendChild(defaultOscillatorVolume);
            oscillatorSettings.AppendChild(defaultOscillatorWaveFormat);

            XmlElement fileTrackSettings = globalConfigs.CreateElement("FILE_TRACK_SETTINGS");
            XmlElement defaultFileTrackGain = globalConfigs.CreateElement("DefaultGain");
            XmlElement defaultFileTrackRepeatMode = globalConfigs.CreateElement("DefaultRepeatMode");
            XmlElement defaultFileTrackTempo = globalConfigs.CreateElement("DefaultTempo");
            XmlElement defaultFileTrackPitch = globalConfigs.CreateElement("DefaultPitch");
            XmlElement defaultFileTrackTimePitchCouplingMode = globalConfigs.CreateElement("DefaultTimePitchCouplingMode");
            defaultFileTrackGain.InnerXml = _FileTrackSettings.DefaultGain.ToString();
            defaultFileTrackRepeatMode.InnerXml = _FileTrackSettings.DefaultRepeatMode.ToString();
            defaultFileTrackTempo.InnerXml = _FileTrackSettings.DefaultTempo.ToString();
            defaultFileTrackPitch.InnerXml = _FileTrackSettings.DefaultPitch.ToString();
            defaultFileTrackTimePitchCouplingMode.InnerXml = _FileTrackSettings.DefaultTimePitchCouplingMode.ToString();
            fileTrackSettings.AppendChild(defaultFileTrackGain);
            fileTrackSettings.AppendChild(defaultFileTrackRepeatMode);
            fileTrackSettings.AppendChild(defaultFileTrackTempo);
            fileTrackSettings.AppendChild(defaultFileTrackPitch);
            fileTrackSettings.AppendChild(defaultFileTrackTimePitchCouplingMode);

            XmlElement midiTrackSettings = globalConfigs.CreateElement("MIDI_TRACK_SETTINGS");
            XmlElement defaultMIDITrackGain = globalConfigs.CreateElement("DefaultGain");
            XmlElement defaultMIDITrackRepeatMode = globalConfigs.CreateElement("DefaultRepeatMode");
            XmlElement defaultMIDITrackOscillatorBaseFrequency = globalConfigs.CreateElement("DefaultOscillatorBaseFrequency");
            XmlElement defaultMIDITrackOscillatorBaseGain = globalConfigs.CreateElement("DefaultOscillatorBaseGain");
            XmlElement defaultMIDITrackOscillatorWaveFormat = globalConfigs.CreateElement("DefaultOscillatorWaveFormat");
            XmlElement defaultMIDITrackTempo = globalConfigs.CreateElement("DefaultFileTrackBaseTempo");
            XmlElement defaultMIDITrackPitch = globalConfigs.CreateElement("DefaultFileTrackBasePitch");
            XmlElement defaultMIDITrackTimePitchCouplingMode = globalConfigs.CreateElement("DefaultTimePitchCouplingMode");
            defaultMIDITrackGain.InnerXml = _MidiTrackSettings.DefaultGain.ToString();
            defaultMIDITrackRepeatMode.InnerXml = _MidiTrackSettings.DefaultRepeatMode.ToString();
            defaultMIDITrackOscillatorBaseFrequency.InnerXml = _MidiTrackSettings.DefaultOscillatorVaseFrequency.ToString();
            defaultMIDITrackOscillatorBaseGain.InnerXml = _MidiTrackSettings.DefaultOscillatorBaseGain.ToString();
            defaultMIDITrackOscillatorWaveFormat.InnerXml = _MidiTrackSettings.DefaultOscillatorWaveFormat.ToString();
            defaultMIDITrackTempo.InnerXml = _MidiTrackSettings.DefaultTempo.ToString();
            defaultMIDITrackPitch.InnerXml = _MidiTrackSettings.DefaultPitch.ToString();
            defaultMIDITrackTimePitchCouplingMode.InnerXml = _MidiTrackSettings.DefaultTimePitchCouplingMode.ToString();
            midiTrackSettings.AppendChild(defaultMIDITrackGain);
            midiTrackSettings.AppendChild(defaultMIDITrackRepeatMode);
            midiTrackSettings.AppendChild(defaultMIDITrackOscillatorBaseFrequency);
            midiTrackSettings.AppendChild(defaultMIDITrackOscillatorBaseGain);
            midiTrackSettings.AppendChild(defaultMIDITrackOscillatorWaveFormat);
            midiTrackSettings.AppendChild(defaultMIDITrackTempo);
            midiTrackSettings.AppendChild(defaultMIDITrackPitch);
            midiTrackSettings.AppendChild(defaultMIDITrackTimePitchCouplingMode);

            XmlElement dspSettings = globalConfigs.CreateElement("DSP_SETTINGS");
            XmlElement distortionSettings = globalConfigs.CreateElement("DISTORTION_SETTINGS");
            XmlElement defaultDistortionDrive = globalConfigs.CreateElement("DefaultDistortionDrive");
            XmlElement defaultDistortionType = globalConfigs.CreateElement("DefaultDistortionType");
            XmlElement compressorSettings = globalConfigs.CreateElement("COMPRESSOR_SETTINGS");
            XmlElement defaultCompressorThreshold = globalConfigs.CreateElement("DefaultThreshold");
            XmlElement defaultCompressorType = globalConfigs.CreateElement("DefaultRatio");
            XmlElement defaultCompressorAttack = globalConfigs.CreateElement("DefaultAttack");
            XmlElement defaultCompressorRelease = globalConfigs.CreateElement("DefaultRelease");
            XmlElement reverbSettings = globalConfigs.CreateElement("REVERB_SETTINGS");
            XmlElement defaultReverbRoomSize = globalConfigs.CreateElement("DefaultRoomSize");
            XmlElement defaultReverbDamping = globalConfigs.CreateElement("DefaultDamping");
            XmlElement defaultReverbWetLevel = globalConfigs.CreateElement("DefaultWetLevel");
            XmlElement defaultReverbDryLevel = globalConfigs.CreateElement("DefaultDryLevel");
            XmlElement defaultReverbWidth = globalConfigs.CreateElement("DefaultWidth");
            XmlElement defaultReverbFreezeMode = globalConfigs.CreateElement("DefaultFreezeMode");
            XmlElement chorusSettings = globalConfigs.CreateElement("CHORUS_SETTINGS");
            XmlElement defaultChorusRate = globalConfigs.CreateElement("DefaultRate");
            XmlElement defaultChorusDepth = globalConfigs.CreateElement("DefaultDepth");
            XmlElement defaultChorusDelay = globalConfigs.CreateElement("DefaultDelay");
            XmlElement defaultChorusFeedback = globalConfigs.CreateElement("DefaultFeedback");
            XmlElement defaultChorusMix = globalConfigs.CreateElement("DefaultMix");
            defaultDistortionDrive.InnerXml = _DspSettings.DistortionSettings.DefaultDistortionDrive.ToString();
            defaultDistortionType.InnerXml = _DspSettings.DistortionSettings.DefaultDistortionType1.ToString();
            defaultCompressorThreshold.InnerXml = _DspSettings.CompressorSettings.DefaultThreshold.ToString();
            defaultCompressorType.InnerXml = _DspSettings.CompressorSettings.DefaultRatio.ToString();
            defaultCompressorAttack.InnerXml = _DspSettings.CompressorSettings.DefaultAttack.ToString();
            defaultCompressorRelease.InnerXml = _DspSettings.CompressorSettings.DefaultRelease.ToString();
            defaultReverbRoomSize.InnerXml = _DspSettings.ReverbSettings.DefaultRoomSize.ToString();
            defaultReverbDamping.InnerXml = _DspSettings.ReverbSettings.DefaultDamping.ToString();
            defaultReverbWetLevel.InnerXml = _DspSettings.ReverbSettings.DefaultWetLevel.ToString();
            defaultReverbDryLevel.InnerXml = _DspSettings.ReverbSettings.DefaultDryLevel.ToString();
            defaultReverbWidth.InnerXml = _DspSettings.ReverbSettings.DefaultWidth.ToString();
            defaultReverbFreezeMode.InnerXml = _DspSettings.ReverbSettings.DefaultFreezeMode.ToString();
            defaultChorusRate.InnerXml = _DspSettings.ChorusSettings.DefaultRate.ToString();
            defaultChorusDepth.InnerXml = _DspSettings.ChorusSettings.DefaultDepth.ToString();
            defaultChorusDelay.InnerXml = _DspSettings.ChorusSettings.DefaultDelay.ToString();
            defaultChorusFeedback.InnerXml = _DspSettings.ChorusSettings.DefaultFeedback.ToString();
            defaultChorusMix.InnerXml = _DspSettings.ChorusSettings.DefaultMix.ToString();
            distortionSettings.AppendChild(defaultDistortionDrive);
            distortionSettings.AppendChild(defaultDistortionType);
            compressorSettings.AppendChild(defaultCompressorThreshold);
            compressorSettings.AppendChild(defaultCompressorType);
            compressorSettings.AppendChild(defaultCompressorAttack);
            compressorSettings.AppendChild(defaultCompressorRelease);
            reverbSettings.AppendChild(defaultReverbRoomSize);
            reverbSettings.AppendChild(defaultReverbDamping);
            reverbSettings.AppendChild(defaultReverbWetLevel);
            reverbSettings.AppendChild(defaultReverbDryLevel);
            reverbSettings.AppendChild(defaultReverbWidth);
            reverbSettings.AppendChild(defaultReverbFreezeMode);
            chorusSettings.AppendChild(defaultChorusRate);
            chorusSettings.AppendChild(defaultChorusDepth);
            chorusSettings.AppendChild(defaultChorusDelay);
            chorusSettings.AppendChild(defaultChorusFeedback);
            chorusSettings.AppendChild(defaultChorusMix);
            dspSettings.AppendChild(distortionSettings);
            dspSettings.AppendChild(compressorSettings);
            dspSettings.AppendChild(reverbSettings);
            dspSettings.AppendChild(chorusSettings);

            root.AppendChild(generalSettings);
            root.AppendChild(oscillatorSettings);
            root.AppendChild(fileTrackSettings);
            root.AppendChild(midiTrackSettings);
            root.AppendChild(dspSettings);

            globalConfigs.Save(configsFilePath);
        }
    }

    public class GeneralSettings
    {
        private float defaultGeneralVolume;

        public float DefaultGeneralVolume { get => defaultGeneralVolume; set => defaultGeneralVolume = value; }
    }

    public class OscillatorSettings
    {
        private float defaultFrequency;
        private float defaultVolume;
        private Global.enumWaveShapeType defaultWaveFormat;

        public float DefaultFrequency { get => defaultFrequency; set => defaultFrequency = value; }
        public float DefaultVolume { get => defaultVolume; set => defaultVolume = value; }
        public enumWaveShapeType DefaultWaveFormat { get => defaultWaveFormat; set => defaultWaveFormat = value; }
    }

    public class FileTrackSettings
    {
        private float defaultGain;
        private bool defaultRepeatMode;
        private float defaultTempo;
        private float defaultPitch;
        private bool defaultTimePitchCouplingMode;

        public float DefaultGain { get => defaultGain; set => defaultGain = value; }
        public bool DefaultRepeatMode { get => defaultRepeatMode; set => defaultRepeatMode = value; }
        public float DefaultTempo { get => defaultTempo; set => defaultTempo = value; }
        public float DefaultPitch { get => defaultPitch; set => defaultPitch = value; }
        public bool DefaultTimePitchCouplingMode { get => defaultTimePitchCouplingMode; set => defaultTimePitchCouplingMode = value; }
    }

    public class MIDITrackSettings
    {
        private float defaultGain;
        private bool defaultRepeatMode;
        private float defaultOscillatorVaseFrequency;
        private float defaultOscillatorBaseGain;
        private Global.enumWaveShapeType defaultOscillatorWaveFormat;
        private float defaultTempo;
        private float defaultPitch;
        private bool defaultTimePitchCouplingMode;

        public float DefaultGain { get => defaultGain; set => defaultGain = value; }
        public bool DefaultRepeatMode { get => defaultRepeatMode; set => defaultRepeatMode = value; }
        public float DefaultOscillatorVaseFrequency { get => defaultOscillatorVaseFrequency; set => defaultOscillatorVaseFrequency = value; }
        public float DefaultOscillatorBaseGain { get => defaultOscillatorBaseGain; set => defaultOscillatorBaseGain = value; }
        public enumWaveShapeType DefaultOscillatorWaveFormat { get => defaultOscillatorWaveFormat; set => defaultOscillatorWaveFormat = value; }
        public float DefaultTempo { get => defaultTempo; set => defaultTempo = value; }
        public float DefaultPitch { get => defaultPitch; set => defaultPitch = value; }
        public bool DefaultTimePitchCouplingMode { get => defaultTimePitchCouplingMode; set => defaultTimePitchCouplingMode = value; }
    }

    public class DSPSettings
    {
        private DistortionSettings distortionSettings = new DistortionSettings();
        private CompressorSettings compressorSettings = new CompressorSettings();
        private ReverbSettings reverbSettings = new ReverbSettings();
        private ChorusSettings chorusSettings = new ChorusSettings();

        public DistortionSettings DistortionSettings { get => distortionSettings; }
        public CompressorSettings CompressorSettings { get => compressorSettings; }
        public ReverbSettings ReverbSettings { get => reverbSettings; }
        public ChorusSettings ChorusSettings { get => chorusSettings; }
    }

    public class DistortionSettings
    {
        private float defaultDistortionDrive;
        private DistortionDSP.enum_distortionType DefaultDistortionType;

        public float DefaultDistortionDrive { get => defaultDistortionDrive; set => defaultDistortionDrive = value; }
        public DistortionDSP.enum_distortionType DefaultDistortionType1 { get => DefaultDistortionType; set => DefaultDistortionType = value; }

        public DistortionSettings() { }
    }

    public class CompressorSettings
    {
        private float defaultThreshold;
        private float defaultRatio;
        private float defaultAttack;
        private float defaultRelease;

        public float DefaultThreshold { get => defaultThreshold; set => defaultThreshold = value; }
        public float DefaultRatio { get => defaultRatio; set => defaultRatio = value; }
        public float DefaultAttack { get => defaultAttack; set => defaultAttack = value; }
        public float DefaultRelease { get => defaultRelease; set => defaultRelease = value; }

        public CompressorSettings() { }
    }

    public class ReverbSettings
    {
        private float defaultRoomSize;
        private float defaultDamping;
        private float defaultWetLevel;
        private float defaultDryLevel;
        private float defaultWidth;
        private bool defaultFreezeMode;

        public float DefaultRoomSize { get => defaultRoomSize; set => defaultRoomSize = value; }
        public float DefaultDamping { get => defaultDamping; set => defaultDamping = value; }
        public float DefaultWetLevel { get => defaultWetLevel; set => defaultWetLevel = value; }
        public float DefaultDryLevel { get => defaultDryLevel; set => defaultDryLevel = value; }
        public float DefaultWidth { get => defaultWidth; set => defaultWidth = value; }
        public bool DefaultFreezeMode { get => defaultFreezeMode; set => defaultFreezeMode = value; }

        public ReverbSettings() { }
    }

    public class ChorusSettings
    {
        private float defaultRate;
        private float defaultDepth;
        private float defaultDelay;
        private float defaultFeedback;
        private float defaultMix;

        public float DefaultRate { get => defaultRate; set => defaultRate = value; }
        public float DefaultDepth { get => defaultDepth; set => defaultDepth = value; }
        public float DefaultDelay { get => defaultDelay; set => defaultDelay = value; }
        public float DefaultFeedback { get => defaultFeedback; set => defaultFeedback = value; }
        public float DefaultMix { get => defaultMix; set => defaultMix = value; }

        public ChorusSettings() { }
    }
}