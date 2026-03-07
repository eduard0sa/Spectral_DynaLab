using SDLab_GUI.UIComponents.StartPageUIComponents;

namespace SDLab_GUI
{
    public partial class StartPage : ContentPage
    {
        MainPage EditorInterfaceInstance = null;

        public StartPage()
        {
            InitializeComponent();
        }

        private void StartEditorEnvironmentEvent(object? sender, EventArgs e)
        {
            if (EditorInterfaceInstance == null)
            {
                EditorInterfaceInstance = new MainPage();
            }

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