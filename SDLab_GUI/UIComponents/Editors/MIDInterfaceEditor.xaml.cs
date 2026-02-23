using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.TrackUIComponents;

namespace SDLab_GUI.UIComponents.Editors;

public partial class MIDIInterfaceEditor : ContentPage
{
    private AudioEngineMGMT audioEngineMGMT;
    private JuceAudioProvider audioProviderOBJ;
    private EventHandler modalBoxCloseEvent;
    bool[] isSharpNodeArr = new bool[] { false, true, false, true, false, false, true, false, true, false, true, false };

    public MIDIInterfaceEditor(AudioEngineMGMT audioManager, JuceAudioProvider audioProvider)
	{
		InitializeComponent();

        // Additional initialization code can be added here
        audioEngineMGMT = audioManager;
        audioProviderOBJ = audioProvider;

        clearMIDIKeyboard();
        generateMIDIKeyboard(MIDIKeyboardVerticalStackLayout);

        Task.Run(test);
        Task<HorizontalStackLayout> newTask = new Task<HorizontalStackLayout>(async () => {
            return await test();
        });

    }

    private async Task<HorizontalStackLayout> test()
    {
        HorizontalStackLayout testHSL = new HorizontalStackLayout();

        for (int i = 0; i < 100; i++)
        {
            VerticalStackLayout Column = new VerticalStackLayout();
            Column.HeightRequest = MIDIKeyboardVerticalStackLayout.Height;
            Column.WidthRequest = 5;
            Column.Margin = new Thickness(10, 0, 0, 0);

            for (int k = 0; k < 6; k++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (isSharpNodeArr[j])
                    {
                        StackLayout blackKey = new StackLayout();
                        blackKey.ZIndex = 2;
                        blackKey.WidthRequest = 10;
                        blackKey.HeightRequest = 20;
                        blackKey.Margin = new Thickness(0, -9, 0, 0);
                        blackKey.BackgroundColor = Color.FromArgb("#151820");
                        Column.Children.Add(blackKey);
                    }
                    else
                    {
                        StackLayout _whiteKey = new StackLayout();
                        _whiteKey.WidthRequest = 10;
                        _whiteKey.HeightRequest = 30;
                        _whiteKey.Margin = new Thickness(0, (j == 5 || j == 0) ? 2 : -9, 0, 0);
                        _whiteKey.BackgroundColor = Color.FromArgb("#101413");
                        Column.Children.Add(_whiteKey);
                    }
                }
            }

            testHSL.Add(Column);
        }

        return testHSL;
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

    private void generateMIDIKeyboard(VerticalStackLayout targetVSL)
    {
        for (int k = 0; k < 6; k++)
        {
            for (int i = 0; i < 12; i++)
            {
                if (isSharpNodeArr[i])
                {
                    StackLayout blackKey = new StackLayout();
                    blackKey.ZIndex = 2;
                    blackKey.HorizontalOptions = LayoutOptions.Start;
                    blackKey.WidthRequest = 70;
                    blackKey.HeightRequest = 20;
                    blackKey.Margin = new Thickness(0, -9, 0, 0);
                    blackKey.BackgroundColor = Colors.Black;
                    targetVSL.Children.Add(blackKey);
                }
                else
                {
                    StackLayout _whiteKey = new StackLayout();
                    _whiteKey.ZIndex = 1;
                    _whiteKey.HorizontalOptions = LayoutOptions.Fill;
                    _whiteKey.HeightRequest = 30;
                    _whiteKey.Margin = new Thickness(0, (i == 5 || i == 0) ? 2 : -9, 0, 0);
                    _whiteKey.BackgroundColor = Colors.White;
                    targetVSL.Children.Add(_whiteKey);
                }
            }
        }

        StackLayout whiteKey = new StackLayout();
        whiteKey.ZIndex = 1;
        whiteKey.HorizontalOptions = LayoutOptions.Fill;
        whiteKey.HeightRequest = 30;
        whiteKey.Margin = new Thickness(0, 2, 0, 0);
        whiteKey.BackgroundColor = Colors.White;
        targetVSL.Children.Add(whiteKey);
    }

    private void clearMIDIKeyboard()
    {
        MIDIKeyboardVerticalStackLayout.Children.Clear();
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
}