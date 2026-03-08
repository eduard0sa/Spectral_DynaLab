using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLab_GUI.UIComponents.StartPageUIComponents
{
    public class ProjectRowItem : Grid
    {
        private Label projectNameLabel;
        private Label projectDateLabel;
        private Label projectPathLabel;

        public ProjectRowItem(string _name, string _creationDate, string _path)
        {
            HorizontalOptions = LayoutOptions.Fill;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = Color.FromArgb("#000003");

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });

            projectNameLabel = new Label() { Text = _name, TextColor = (Color)Application.Current.Resources["DefaultPastelRed"], FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            projectDateLabel = new Label() { Text = _creationDate, TextColor = (Color)Application.Current.Resources["DefaultPastelRed"], FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            projectPathLabel = new Label() { Text = _path, TextColor = (Color)Application.Current.Resources["DefaultPastelRed"], FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };

            Children.Add(projectNameLabel);
            Children.Add(projectDateLabel);
            Children.Add(projectPathLabel);

            Grid.SetColumn(projectNameLabel, 0);
            Grid.SetColumn(projectDateLabel, 1);
            Grid.SetColumn(projectPathLabel, 2);
        }
    }
}