using SDLab_GUI.UIComponents.StartPageUIComponents;
using SDLab_GUI.Configurations;
using System.Xml;
using SDLab_GUI.UIComponents.Editors;

namespace SDLab_GUI
{
    public partial class StartPage : ContentPage
    {
        MainPage EditorInterfaceInstance = null;
        GlobalConfigs globalConfigurationSet;
        string configsFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SPECTRAL_DYNALab/configs/global_configs.xml";

        string editorConfigsFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SPECTRAL_DYNALab/configs/editor_settings.xml";
        EditorConfigs editorConfigurationSet;

        SettingsModalEditor settingsEditorModal;

        public StartPage()
        {
            InitializeComponent();

            checkGlobalConfigsFileExistance();

            globalConfigurationSet = new GlobalConfigs(configsFilePath);
            globalConfigurationSet.loadConfigsFromFile();

            if(globalConfigurationSet.FirstOpened.Date.Year == 3301)
            {
                globalConfigurationSet.FirstOpened = DateTime.Now;
            }

            globalConfigurationSet.LastOpened = DateTime.Now;

            globalConfigurationSet.updateGlobalConfigsXML();

            loadRecentProjectUIList();

            checkEditorConfigsFileExistance();

            editorConfigurationSet = new EditorConfigs(editorConfigsFilePath);
            editorConfigurationSet.loadConfigsFromFile();
        }

        private void showMainDashboard(object? sender, EventArgs e)
        {
            StartPageMainDashboard.IsVisible = true;
            StartPageProjectsDashboard.IsVisible = false;

            Color newColor1 = (Color)Application.Current.Resources["DarkPastelRed"];
            if (newColor1 != null) StartPageMainDashboardBTN.BackgroundColor = newColor1;

            Color newColor2 = Colors.Transparent;
            if (newColor2 != null) StartPageProjectsDashboardBTN.BackgroundColor = newColor2;
        }

        private void showProjectsDashboard(object? sender, EventArgs e)
        {
            StartPageMainDashboard.IsVisible = false;
            StartPageProjectsDashboard.IsVisible = true;

            Color newColor1 = Colors.Transparent;
            if (newColor1 != null) StartPageMainDashboardBTN.BackgroundColor = newColor1;

            Color newColor2 = (Color)Application.Current.Resources["DarkPastelRed"];
            if (newColor2 != null) StartPageProjectsDashboardBTN.BackgroundColor = newColor2;
        }

        private void checkGlobalConfigsFileExistance()
        {
            if (!File.Exists(configsFilePath))
            {
                XmlDocument globalConfigs = new XmlDocument();
                globalConfigs.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<GLOBAL_CONFIGS>\r\n\t<FirstOpened Date=\"3301-01-01\" Time=\"00:00:00\"/>\r\n\t<LastOpened Date=\"2024-06-20\" Time=\"15:30:00\"/>\r\n\t<RecentProjects>\r\n\t\t<Project>\r\n\t\t\t<Name>Projecto de Teste</Name>\r\n\t\t\t<Path>C:\\Users\\Utilizador\\Desktop\\SDLab_GUI\\TestProject.sdproj</Path>\r\n\t\t\t<CreationDateTime Date=\"2024-06-20\" Time=\"15:30:00\"/>\r\n\t\t\t<LastOpenedDateTime Date=\"2024-06-20\" Time=\"15:30:00\"/>\r\n\t\t</Project>\r\n\t\t<Project>\r\n\t\t\t<Name>Outro Projecto de Teste</Name>\r\n\t\t\t<Path>C:\\Users\\Utilizador\\Desktop\\SDLab_GUI\\TestProject2.sdproj</Path>\r\n\t\t\t<CreationDateTime Date=\"2024-06-20\" Time=\"15:30:00\"/>\r\n\t\t\t<LastOpenedDateTime Date=\"2024-06-20\" Time=\"15:30:00\"/>\r\n\t\t</Project>\r\n\t</RecentProjects>\r\n</GLOBAL_CONFIGS>");

                if (!Directory.Exists(Path.GetDirectoryName(configsFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configsFilePath));
                }

                globalConfigs.Save(configsFilePath);
            }
        }

        private void checkEditorConfigsFileExistance()
        {
            if (!File.Exists(configsFilePath))
            {
                XmlDocument globalConfigs = new XmlDocument();
                globalConfigs.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<EDITOR_CONFIGS>\r\n\t<GENERAL_SETTINGS>\r\n\t\t<DefaultGeneralVolume>100</DefaultGeneralVolume>\r\n\t</GENERAL_SETTINGS>\r\n\t\r\n\t<OSCILLATOR_SETTINGS>\r\n\t\t<DefaultFrequency>30</DefaultFrequency>\r\n\t\t<DefaultGain>0.50</DefaultGain>\r\n\t\t<DefaultWaveFormat>Sine</DefaultWaveFormat>\r\n\t</OSCILLATOR_SETTINGS>\r\n\r\n\t<FILE_TRACK_SETTINGS>\r\n\t\t<DefaultGain>0.50</DefaultGain>\r\n\t\t<DefaultRepeatMode>true</DefaultRepeatMode>\r\n\t\t<DefaultTempo>1.0</DefaultTempo>\r\n\t\t<DefaultPitch>1.0</DefaultPitch>\r\n\t\t<DefaultTimePitchCouplingMode>true</DefaultTimePitchCouplingMode>\r\n\t</FILE_TRACK_SETTINGS>\r\n\r\n\t<MIDI_TRACK_SETTINGS>\r\n\t\t<DefaultGain>0.50</DefaultGain>\r\n\t\t<DefaultRepeatMode>true</DefaultRepeatMode>\r\n\t\t<DefaultOscillatorBaseFrequency>262.63</DefaultOscillatorBaseFrequency>\r\n\t\t<DefaultOscillatorBaseGain>0.50</DefaultOscillatorBaseGain>\r\n\t\t<DefaultOscillatorWaveFormat>Sine</DefaultOscillatorWaveFormat>\r\n\t\t<DefaultFileTrackBaseTempo>1.0</DefaultFileTrackBaseTempo>\r\n\t\t<DefaultFileTrackBasePitch>1.0</DefaultFileTrackBasePitch>\r\n\t\t<DefaultFileTrackBaseTimePitchCouplingMode>true</DefaultFileTrackBaseTimePitchCouplingMode>\r\n\t</MIDI_TRACK_SETTINGS>\r\n\t\r\n\t<DSP_SETTINGS>\r\n\t\t<DISTORTION_SETTINGS>\r\n\t\t\t<DefaultDistortionDrive>10.0</DefaultDistortionDrive>\r\n\t\t\t<DefaultDistortionType>SoftClip</DefaultDistortionType>\r\n\t\t</DISTORTION_SETTINGS>\r\n\t\t\r\n\t\t<COMPRESSOR_SETTINGS>\r\n\t\t\t<DefaultThreshold>-10</DefaultThreshold>\r\n\t\t\t<DefaultRatio>2.0</DefaultRatio>\r\n\t\t\t<DefaultAttack>20.00</DefaultAttack>\r\n\t\t\t<DefaultRelease>500.00</DefaultRelease>\r\n\t\t</COMPRESSOR_SETTINGS>\r\n\t\t\r\n\t\t<REVERB_SETTINGS>\r\n\t\t\t<DefaultRoomSize>0.50</DefaultRoomSize>\r\n\t\t\t<DefaultDamping>0.50</DefaultDamping>\r\n\t\t\t<DefaultWetLevel>0.50</DefaultWetLevel>\r\n\t\t\t<DefaultDryLevel>1.0</DefaultDryLevel>\r\n\t\t\t<DefaultWidth>0.50</DefaultWidth>\r\n\t\t\t<DefaultFreezeMode>false</DefaultFreezeMode>\r\n\t\t</REVERB_SETTINGS>\r\n\r\n\t\t<CHORUS_SETTINGS>\r\n\t\t\t<DefaultRate>0.80</DefaultRate>\r\n\t\t\t<DefaultDepth>0.40</DefaultDepth>\r\n\t\t\t<DefaultDelay>25.00</DefaultDelay>\r\n\t\t\t<DefaultFeedback>0.00</DefaultFeedback>\r\n\t\t\t<DefaultMix>0.50</DefaultMix>\r\n\t\t</CHORUS_SETTINGS>\r\n\t</DSP_SETTINGS>\r\n</EDITOR_CONFIGS>");

                if (!Directory.Exists(Path.GetDirectoryName(configsFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configsFilePath));
                }

                globalConfigs.Save(configsFilePath);
            }
        }

        private void loadRecentProjectUIList() {
            for(int i = 0; i < globalConfigurationSet.RecentProjects.Count; i++)
            {
                ProjectRowItem projectRowItem = new ProjectRowItem(globalConfigurationSet.RecentProjects[i].Name, $"{globalConfigurationSet.RecentProjects[i].CreationDateTime.ToShortDateString()} {globalConfigurationSet.RecentProjects[i].CreationDateTime.ToShortTimeString()}", globalConfigurationSet.RecentProjects[i].Path);
                StackLayout bottomSeparatorBorder = new StackLayout() { HeightRequest = 2, BackgroundColor = Colors.White };

                RecentSetsFlexLayout.Children.Add(projectRowItem);
                RecentSetsFlexLayout.Children.Add(bottomSeparatorBorder);
            }
        }

        private void StartEditorEnvironmentEvent(object? sender, EventArgs e)
        {
            EditorInterfaceInstance = new MainPage(enumEditorMode.Default);

            if(EditorInterfaceInstance.CurrentEditorMode == enumEditorMode.Tutorial)
            {
                EditorInterfaceInstance = new MainPage(enumEditorMode.Default);
            }

            ShowEditorEnvironmentBTN.IsVisible = true;

            Navigation.PushAsync(EditorInterfaceInstance);
        }

        private void ShowEditorEnvironmentEvent(object? sender, EventArgs e)
        {
            Navigation.PushAsync(EditorInterfaceInstance);
        }

        private void StartEditorTutorialEvent(object? sender, EventArgs e)
        {
            EditorInterfaceInstance = new MainPage(enumEditorMode.Tutorial);

            Navigation.PushAsync(EditorInterfaceInstance);
        }

        private void OpenSettingsModal(object? sender, EventArgs e)
        {
            settingsEditorModal = new SettingsModalEditor(editorConfigurationSet);
            Navigation.PushModalAsync(settingsEditorModal);
        }

        /// <summary>
        /// This method automatically unfocuses a slider element when it is focus, in order to avoid it from focusing on app start.
        /// </summary>
        /// <param name="sender">Slider sender object.</param>
        /// <param name="e">Slider focus EventArguments.</param>
        private void ButtonAutoUnfocusEvent(object? sender, FocusEventArgs e)
        {
            Button originButton = sender as Button;
            originButton.Unfocus();
        }
    }
}