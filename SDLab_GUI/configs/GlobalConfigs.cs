using System.Xml;

namespace SDLab_GUI.Configurations
{
    public class GlobalConfigs : IConfigEntity
    {
        private DateTime firstOpened = new DateTime();
        private DateTime lastOpened = new DateTime();
        private List<ProjectRegistryData> recentProjects;
        string configsFilePath;

        public DateTime FirstOpened { get => firstOpened; set => firstOpened = value; }
        public DateTime LastOpened { get => lastOpened; set => lastOpened = value; }
        public List<ProjectRegistryData> RecentProjects { get => recentProjects; set => recentProjects = value; }

        public GlobalConfigs(string _configsFilePath)
        {
            configsFilePath = _configsFilePath;
        }

        public void loadConfigsFromFile()
        {
            XmlDocument globalConfigs = new XmlDocument();

            globalConfigs.Load(configsFilePath);

            DateTime firstOpenedDateTime = new DateTime();
            DateTime lastOpenedDateTime = new DateTime();
            List<ProjectRegistryData> recentProjectsList = new List<ProjectRegistryData>();

            if (globalConfigs.DocumentElement.ChildNodes.Count == 3)
            {
                firstOpenedDateTime = DateTime.Parse($"{globalConfigs.DocumentElement.ChildNodes[0].Attributes[0].InnerText} {globalConfigs.DocumentElement.ChildNodes[0].Attributes[1].InnerText}");
                lastOpenedDateTime = DateTime.Parse($"{globalConfigs.DocumentElement.ChildNodes[1].Attributes[0].InnerText} {globalConfigs.DocumentElement.ChildNodes[1].Attributes[1].InnerText}");
                XmlNodeList recentProjects;

                recentProjects = globalConfigs.DocumentElement.ChildNodes[2].ChildNodes;

                for (int i = 0; i < recentProjects.Count; i++)
                {
                    ProjectRegistryData projectRegistryData = new ProjectRegistryData()
                    {
                        Name = recentProjects[i].ChildNodes[0].InnerXml,
                        Path = recentProjects[i].ChildNodes[1].InnerXml,
                        CreationDateTime = DateTime.Parse($"{recentProjects[i].ChildNodes[2].Attributes[0].InnerText} {recentProjects[i].ChildNodes[2].Attributes[1].InnerText}"),
                        LastOpenedDateTime = DateTime.Parse($"{recentProjects[i].ChildNodes[3].Attributes[0].InnerText} {recentProjects[i].ChildNodes[3].Attributes[1].InnerText}")
                    };

                    recentProjectsList.Add(projectRegistryData);
                }
            }


            FirstOpened = firstOpenedDateTime;
            LastOpened = lastOpenedDateTime;
            RecentProjects = recentProjectsList;
        }

        public void updateGlobalConfigsXML()
        {
            XmlDocument globalConfigs = new XmlDocument();

            XmlElement root = globalConfigs.CreateElement("GLOBAL_CONFIGS");
            globalConfigs.AppendChild(root);

            XmlElement firstOpened = globalConfigs.CreateElement("FirstOpened");
            firstOpened.SetAttribute("Date", FirstOpened.ToShortDateString());
            firstOpened.SetAttribute("Time", FirstOpened.ToShortTimeString());

            XmlElement lastOpened = globalConfigs.CreateElement("LastOpened");
            lastOpened.SetAttribute("Date", LastOpened.ToShortDateString());
            lastOpened.SetAttribute("Time", LastOpened.ToShortTimeString());

            XmlElement recentProjects = globalConfigs.CreateElement("RecentProjects");

            for (int i = 0; i < RecentProjects.Count; i++)
            {
                XmlElement project = globalConfigs.CreateElement("Project");

                XmlElement projectName = globalConfigs.CreateElement("Name");
                projectName.InnerXml = RecentProjects[i].Name;

                XmlElement projectPath = globalConfigs.CreateElement("Path");
                projectPath.InnerXml = RecentProjects[i].Path;

                XmlElement projectCreationDateTime = globalConfigs.CreateElement("CreationDateTime");
                projectCreationDateTime.SetAttribute("Date", RecentProjects[i].CreationDateTime.ToShortDateString());
                projectCreationDateTime.SetAttribute("Time", RecentProjects[i].CreationDateTime.ToShortTimeString());

                XmlElement projectLastOpenedDateTime = globalConfigs.CreateElement("LastOpenedDateTime");
                projectLastOpenedDateTime.SetAttribute("Date", RecentProjects[i].LastOpenedDateTime.ToShortDateString());
                projectLastOpenedDateTime.SetAttribute("Time", RecentProjects[i].LastOpenedDateTime.ToShortTimeString());

                project.AppendChild(projectName);
                project.AppendChild(projectPath);
                project.AppendChild(projectCreationDateTime);
                project.AppendChild(projectLastOpenedDateTime);

                recentProjects.AppendChild(project);
            }

            root.AppendChild(firstOpened);
            root.AppendChild(lastOpened);
            root.AppendChild(recentProjects);

            globalConfigs.Save(configsFilePath);
        }
    }

    public class ProjectRegistryData
    {
        private string name;
        private string path;
        private DateTime creationDateTime;
        private DateTime lastOpenedDateTime;

        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
        public DateTime CreationDateTime { get => creationDateTime; set => creationDateTime = value; }
        public DateTime LastOpenedDateTime { get => lastOpenedDateTime; set => lastOpenedDateTime = value; }
    }
}