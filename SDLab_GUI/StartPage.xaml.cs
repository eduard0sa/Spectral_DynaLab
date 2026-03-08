using SDLab_GUI.UIComponents.StartPageUIComponents;
using SDLab_GUI.Configurations;
using System.Xml;

namespace SDLab_GUI
{
    public partial class StartPage : ContentPage
    {
        MainPage EditorInterfaceInstance = null;
        GlobalConfigs globalConfigurationSet;
        string configsFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SPECTRAL_DYNALab/configs/global_configs.xml";

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

        private void loadRecentProjectUIList() {
            for(int i = 0; i < globalConfigurationSet.RecentProjects.Count; i++)
            {
                ProjectRowItem projectRowItem = new ProjectRowItem(globalConfigurationSet.RecentProjects[i].Name, $"{globalConfigurationSet.RecentProjects[i].CreationDateTime.ToShortDateString()} {globalConfigurationSet.RecentProjects[i].CreationDateTime.ToShortTimeString()}", globalConfigurationSet.RecentProjects[i].Path);
                StackLayout bottomSeparatorBorder = new StackLayout() { HeightRequest = 2, BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"] };

                RecentSetsFlexLayout.Children.Add(projectRowItem);
                RecentSetsFlexLayout.Children.Add(bottomSeparatorBorder);
            }
        }

        private void StartEditorEnvironmentEvent(object? sender, EventArgs e)
        {
            if (EditorInterfaceInstance == null)
            {
                EditorInterfaceInstance = new MainPage(enumEditorMode.Default);
            }

            Navigation.PushAsync(EditorInterfaceInstance);
        }

        private void StartEditorTutorialEvent(object? sender, EventArgs e)
        {
            EditorInterfaceInstance = new MainPage(enumEditorMode.Tutorial);

            Navigation.PushAsync(EditorInterfaceInstance);
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