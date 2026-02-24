using Microsoft.Maui.Graphics;
using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.TrackUIComponents;
using System.Data.Common;

namespace SDLab_GUI.UIComponents.Editors;

public partial class MIDIInterfaceEditor : ContentPage
{
    private AudioEngineMGMT audioEngineMGMT;
    private JuceAudioProvider audioProviderOBJ;
    private EventHandler modalBoxCloseEvent;
    private PianoRollGraphicsView pianoRoll;

    bool[] isSharpNodeArr = new bool[] { false, true, false, true, false, false, true, false, true, false, true, false };

    private struct struct_timelineInfo
    {
        public float timeSpan;
        public float timeUnitSquareWidth;
    }

    private struct struct_keyboardInfo
    {
        public int numWhiteKeysPerOctave;
        public int numOctaves;
        public bool[] isSharpNoteArrRef;
    }

    public MIDIInterfaceEditor(AudioEngineMGMT audioManager, JuceAudioProvider audioProvider)
	{
		InitializeComponent();

        // Additional initialization code can be added here
        audioEngineMGMT = audioManager;
        audioProviderOBJ = audioProvider;

        clearMIDIKeyboard();
        generateMIDIKeyboard(MIDIKeyboardVerticalStackLayout);

        struct_timelineInfo timelineInfo = new struct_timelineInfo()
        {
            timeSpan = 200, //In blocks of 100 milliseconds
            timeUnitSquareWidth = 10
        };

        struct_keyboardInfo keyboardInfo = new struct_keyboardInfo()
        {
            numWhiteKeysPerOctave = 12,
            numOctaves = 6,
            isSharpNoteArrRef = isSharpNodeArr
        };

        MIDIPianoRollStackLayout.HeightRequest = MIDIKeyboardVerticalStackLayout.Height;

        pianoRoll = new PianoRollGraphicsView(MIDIPianoRollStackLayout, timelineInfo, keyboardInfo);
        MIDIKeyboardScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        MIDIKeyboardScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

        MIDIPianoRollStackLayout.Children.Add(pianoRoll);

        MIDIPianoRollScrollView.Scrolled += MIDIEditorScrolledEvent;
        MIDIKeyboardScrollView.Scrolled += MIDIEditorScrolledEvent;
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

    private void MIDIEditorScrolledEvent(object? sender, ScrolledEventArgs e)
    {
        MIDIKeyboardScrollView.ScrollToAsync(x: MIDIKeyboardScrollView.ScrollX, y: MIDIPianoRollScrollView.ScrollY, animated: true);
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

    class PianoRollGraphicsView : GraphicsView
    {
        PianoRollDrawable drawable = new PianoRollDrawable();

        IDispatcherTimer updateFrameTimer;

        /// <summary>
        /// This property hold the function that updates the sound wave visualizer graph, every 0.033s (30 FPS).
        /// </summary>
        public IDispatcherTimer UpdateFrameTimer
        {
            get => updateFrameTimer;
            set => updateFrameTimer = value;
        }

        public PianoRollGraphicsView(StackLayout parent, struct_timelineInfo timeLineInfo, struct_keyboardInfo keyboardInfo)
        {
            WidthRequest = timeLineInfo.timeUnitSquareWidth * timeLineInfo.timeSpan;
            HeightRequest = 20 * (keyboardInfo.numWhiteKeysPerOctave * keyboardInfo.numOctaves + 1) + 2 * keyboardInfo.numWhiteKeysPerOctave * keyboardInfo.numOctaves;
            HorizontalOptions = LayoutOptions.Start;
            drawable.KI = keyboardInfo;
            drawable.TI = timeLineInfo;

            BackgroundColor = (Color)Application.Current.Resources["Gray500"];

            Drawable = drawable;

            UpdateFrameTimer = Dispatcher.CreateTimer();

            UpdateFrameTimer.Interval = TimeSpan.FromMilliseconds(33);

            UpdateFrameTimer.Tick += delegate {
                Invalidate();
            };

            UpdateFrameTimer.Start();
        }
    }
    
    class PianoRollDrawable : IDrawable
    {
        struct_keyboardInfo _KI;
        struct_timelineInfo _TI;

        // Example notes: (noteIndex = 0..71), timeIndex = 0..63
        public List<(int noteIndex, int timeIndex)> ActiveNotes = new();

        public struct_keyboardInfo KI { get => _KI; set => _KI = value; }
        public struct_timelineInfo TI { get => _TI; set => _TI = value; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            float y = 0;

            int noteIndex = 0;

            for (int note = 0; note < KI.numOctaves * KI.numWhiteKeysPerOctave + 1; note++)
            {
                if (KI.isSharpNoteArrRef[noteIndex])
                {
                    canvas.StrokeColor = Colors.Gray;
                    canvas.DrawLine(0, y, TI.timeSpan * TI.timeUnitSquareWidth, y);
                    y += 22;
                }
                else
                {
                    canvas.StrokeColor = Colors.Gray;
                    canvas.DrawLine(0, y, TI.timeSpan * TI.timeUnitSquareWidth, y);
                    y += 22;
                }

                if (noteIndex == KI.numWhiteKeysPerOctave - 1) {
                    noteIndex = 0;
                    continue;
                }

                noteIndex++;
            }

            for (int t = 0; t <= TI.timeSpan; t++)
            {
                canvas.StrokeColor = Colors.DarkGray;
                canvas.DrawLine(t * TI.timeUnitSquareWidth, 0, t * TI.timeUnitSquareWidth, (KI.numOctaves * KI.numWhiteKeysPerOctave + 1) * 22);
            }

            // Draw active notes
            foreach (var (_noteIndex, timeIndex) in ActiveNotes)
            {
                float rectY = _noteIndex * 32;
                float rectX = timeIndex * TI.timeUnitSquareWidth;

                canvas.FillColor = Colors.Green;
                canvas.FillRectangle(rectX, rectY, TI.timeUnitSquareWidth, 32);
            }
        }
    }
}