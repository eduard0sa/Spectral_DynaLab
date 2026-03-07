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

        public ProjectRowItem()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Padding = new Thickness(20, 20, 20, 20);
            BackgroundColor = Color.FromArgb("#000003");

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(33, GridUnitType.Star) });

            projectNameLabel = new Label() { TextColor = (Color)Application.Current.Resources["DefaultPastelRed"] };
            projectDateLabel = new Label() { TextColor = (Color)Application.Current.Resources["DefaultPastelRed"] };
            projectPathLabel = new Label() { TextColor = (Color)Application.Current.Resources["DefaultPastelRed"] };

            Children.Add(projectNameLabel);
            Children.Add(projectDateLabel);
            Children.Add(projectPathLabel);
    
            Grid.SetColumn(projectNameLabel, 0);
            Grid.SetColumn(projectDateLabel, 1);
            Grid.SetColumn(projectPathLabel, 2);
        }
    }
}