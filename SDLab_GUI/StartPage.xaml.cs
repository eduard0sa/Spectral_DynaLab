namespace SDLab_GUI;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

	private void StartEditorEnvironmentEvent(object? sender, EventArgs e)
	{
		Navigation.PushAsync(new MainPage());
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