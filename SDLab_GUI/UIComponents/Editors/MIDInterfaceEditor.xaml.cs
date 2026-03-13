using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.TrackUIComponents;

namespace SDLab_GUI.UIComponents.Editors;

struct struct_timelineInfo
{
    public float timeSpan;
    public float timeUnitSquareWidth;
}

struct struct_keyboardInfo
{
    public int numWhiteKeysPerOctave;
    public int numOctaves;
    public bool[] isSharpNoteArrRef;
    public float[] pianoRollLinesHeight;
}

public partial class MIDIInterfaceEditor : ContentPage
{
    private AudioEngineMGMT audioEngineMGMT;
    private MIDITrackItem MIDIAudioTrack;
    private EventHandler modalBoxCloseEvent;
    private PianoRollGraphicsView pianoRoll;
    private MainPage mainPageRefObj;

    bool[] isSharpNodeArr = new bool[] { false, true, false, true, false, false, true, false, true, false, true, false };
    float[] pianoRollLinesHeight = new float[] { 22f, 20f, 12f, 20f, 22f, 22f, 20f, 12f, 20f, 12f, 20f, 22f };

    public MIDIInterfaceEditor(MainPage _mainPageRef, AudioEngineMGMT audioManager, MIDITrackItem audioProvider, TrackItem templateSampleAudioProvider)
	{
		InitializeComponent();

        // Additional initialization code can be added here
        audioEngineMGMT = audioManager;
        MIDIAudioTrack = (MIDITrackItem)audioProvider;
        mainPageRefObj = _mainPageRef;

        clearMIDIKeyboard();
        generateMIDIKeyboard(MIDIKeyboardVerticalStackLayout);

        struct_timelineInfo timelineInfo = new struct_timelineInfo()
        {
            timeSpan = 200, //In blocks of 100 milliseconds
            timeUnitSquareWidth = 20
        };

        struct_keyboardInfo keyboardInfo = new struct_keyboardInfo()
        {
            numWhiteKeysPerOctave = 12,
            numOctaves = 6,
            isSharpNoteArrRef = isSharpNodeArr,
            pianoRollLinesHeight = pianoRollLinesHeight
        };

        MIDIPianoRollStackLayout.HeightRequest = MIDIKeyboardVerticalStackLayout.Height;

        pianoRoll = new PianoRollGraphicsView(MIDIPianoRollStackLayout, timelineInfo, keyboardInfo, MIDIAudioTrack.TrackAudioProvider);
        MIDIKeyboardScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        MIDIKeyboardScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

        MIDIPianoRollStackLayout.Children.Add(pianoRoll);

        MIDIPianoRollScrollView.Scrolled += MIDIEditorScrolledEvent;
        MIDIKeyboardScrollView.Scrolled += MIDIEditorScrolledEvent;

        if(templateSampleAudioProvider != null)
        {
            addMIDITrackTemplateComponentUI(templateSampleAudioProvider);
        }

        this.Loaded += PageLoadedEvent;
    }

    private async void PageLoadedEvent(object? sender, EventArgs e)
    {
        // Get the visible viewport height
        double viewportHeight = MIDIPianoRollScrollView.Height;

        // Get the total content height
        double contentHeight = MIDIPianoRollStackLayout.Height;

        // Calculate the center position accounting for viewport size
        double centerScrollPosition = (contentHeight / 2) - (viewportHeight / 2);

        // Clamp to valid range (0 to max scrollable distance)
        centerScrollPosition = Math.Max(0, Math.Min(centerScrollPosition, contentHeight - viewportHeight));

        await MIDIPianoRollScrollView.ScrollToAsync(MIDIPianoRollScrollView.ScrollX, centerScrollPosition, animated: false);

        MIDIConfigureOscilatorSignal.Command = new Command(() => { SetMIDIOscillatorTrackTemplateAP(new object(), new EventArgs()); });
        MIDIConfigureExternalSample.Command = new Command(() => { SetMIDIFileTrackTemplateAP(new object(), new EventArgs()); });
    }

    /// <summary>
    /// This property hold the function that closes the modal box on user click.
    /// </summary>
    internal EventHandler ModalBoxCloseEvent
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

    private async void SetMIDIOscillatorTrackTemplateAP(object? sender, EventArgs e)
    {
        TrackItem newTemplateAP = await MIDIAudioTrack.SetMIDITrackTemplateAP(Global.enumEngineType.Oscillator);
        addMIDITrackTemplateComponentUI(newTemplateAP);
        ((MIDITrackProvider)MIDIAudioTrack.TrackAudioProvider).SetMIDITemplateSamplingProvider(newTemplateAP.TrackAudioProvider);
    }

    private async void SetMIDIFileTrackTemplateAP(object? sender, EventArgs e)
    {
        TrackItem newTemplateAP = await MIDIAudioTrack.SetMIDITrackTemplateAP(Global.enumEngineType.FileTrack);
        if(newTemplateAP == null) return;
        addMIDITrackTemplateComponentUI(newTemplateAP);
        ((MIDITrackProvider)MIDIAudioTrack.TrackAudioProvider).SetMIDITemplateSamplingProvider(newTemplateAP.TrackAudioProvider);
    }

    private void addMIDITrackTemplateComponentUI(TrackItem newTemplateAP)
    {
        if (newTemplateAP != null)
        {
            MIDITemplateAudioProviderLayout.Children.Add(newTemplateAP);

            MIDINoTemplateAudioProviderBanner.IsVisible = false;
            MIDITemplateAudioProviderLayout.IsVisible = true;

            mainPlayBTN.Clicked += async delegate {
                mainPageRefObj.ShowLoadingModalSplashScreen();

                switch (mainPlayBTN.Source.ToString().Split(" ")[1])
                {
                    case "pause_button.png":
                        mainPlayBTN.Source = "play_solid_full.png";
                        ((MIDITrackProvider)MIDIAudioTrack.TrackAudioProvider).PauseMIDI();
                        break;

                    case "play_solid_full.png":
                        mainPlayBTN.Source = "pause_button.png";
                        await Task.Run(() => {
                            ((MIDITrackProvider)MIDIAudioTrack.TrackAudioProvider).renderMIDIWaveform(newTemplateAP.TrackAudioProvider);
                            ((MIDITrackProvider)MIDIAudioTrack.TrackAudioProvider).PlayMIDI();
                        });
                        break;
                }

                mainPageRefObj.HideLoadingModalSplashScreen();
            };
        }
    }
}

class PianoRollGraphicsView : GraphicsView
{
    PianoRollDrawable drawable = new PianoRollDrawable();
    PointerGestureRecognizer pointerGesture = new PointerGestureRecognizer();
    MIDITrackProvider MIDIAP;

    IDispatcherTimer updateFrameTimer;

    /// <summary>
    /// This property hold the function that updates the sound wave visualizer graph, every 0.033s (30 FPS).
    /// </summary>
    public IDispatcherTimer UpdateFrameTimer
    {
        get => updateFrameTimer;
        set => updateFrameTimer = value;
    }

    public PianoRollGraphicsView(StackLayout parent, struct_timelineInfo timeLineInfo, struct_keyboardInfo keyboardInfo, JuceAudioProvider MIDIAP)
    {
        float height = 0;

        for (int i = 0; i < 12; i++)
        {
            height += keyboardInfo.pianoRollLinesHeight[i];
        }

        height *= keyboardInfo.numOctaves;
        height += 22;

        WidthRequest = timeLineInfo.timeUnitSquareWidth * timeLineInfo.timeSpan;
        HeightRequest = height;
        HorizontalOptions = LayoutOptions.Start;
        drawable.KI = keyboardInfo;
        drawable.TI = timeLineInfo;

        BackgroundColor = (Color)Application.Current.Resources["Gray500"];

        drawable.MIDIAP = MIDIAP;

        this.StartInteraction += OnStartInteraction;
        this.DragInteraction += OnStartInteraction;
        pointerGesture.PointerMoved += OnHoverInteraction;
        this.GestureRecognizers.Add(pointerGesture);

        Drawable = drawable;

        UpdateFrameTimer = Dispatcher.CreateTimer();

        UpdateFrameTimer.Interval = TimeSpan.FromMilliseconds(33);

        UpdateFrameTimer.Tick += delegate {
            Invalidate();
        };

        UpdateFrameTimer.Start();
    }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        var point = e.Touches.First();

        float x = (float)point.X;
        float y = (float)point.Y;

        int timeIndex = (int)(x / drawable.TI.timeUnitSquareWidth);

        int heightClicked = 0, i = 0, c = 0;
        while (heightClicked < y)
        {
            heightClicked += (int)drawable.KI.pianoRollLinesHeight[i];
            i++;
            c++;
            if (i == 12) i = 0;
        }

        int noteIndex = c - 1;

        Global.struct_coordinates targetNoteCoordinates = new Global.struct_coordinates()
        {
            x = timeIndex,
            y = noteIndex
        };

        drawable.ToggleNote(targetNoteCoordinates);

        Invalidate();
    }

    private void OnHoverInteraction(object sender, PointerEventArgs e)
    {
        var point = e.GetPosition(this);

        float x = (float)point.Value.X;
        float y = (float)point.Value.Y;

        int timeIndex = (int)(x / drawable.TI.timeUnitSquareWidth);

        int heightClicked = 0, i = 0, c = 0;
        while (heightClicked < y)
        {
            heightClicked += (int)drawable.KI.pianoRollLinesHeight[i];
            i++;
            c++;
            if (i == 12) i = 0;
        }

        int noteIndex = c - 1;

        Global.struct_coordinates targetNoteCoordinates = new Global.struct_coordinates()
        {
            x = timeIndex,
            y = noteIndex
        };

        drawable.HoverNote(targetNoteCoordinates);

        Invalidate();
    }

    class PianoRollDrawable : IDrawable
    {
        struct_keyboardInfo _KI;
        struct_timelineInfo _TI;
        JuceAudioProvider _MIDIAP;

        private Global.struct_coordinates hoveredNote = new Global.struct_coordinates() { x = -1, y = -1 };

        public struct_keyboardInfo KI { get => _KI; set => _KI = value; }
        public struct_timelineInfo TI { get => _TI; set => _TI = value; }
        public JuceAudioProvider MIDIAP { get => _MIDIAP; set => _MIDIAP = value; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            float y = 0;

            int noteIndex = 0;

            for (int note = 0; note < KI.numOctaves * KI.numWhiteKeysPerOctave + 1; note++)
            {
                canvas.StrokeColor = Color.FromArgb("#252526");
                canvas.DrawLine(0, y, TI.timeSpan * TI.timeUnitSquareWidth, y);
                y += KI.pianoRollLinesHeight[noteIndex];

                if (noteIndex == KI.numWhiteKeysPerOctave - 1)
                {
                    noteIndex = 0;
                    continue;
                }

                noteIndex++;
            }

            for (int t = 0; t <= TI.timeSpan; t++)
            {
                canvas.StrokeColor = Color.FromArgb("#252526");
                canvas.DrawLine(t * TI.timeUnitSquareWidth, 0, t * TI.timeUnitSquareWidth, (KI.numOctaves * KI.numWhiteKeysPerOctave + 1) * 22);
            }

            // Draw active notes
            foreach (Global.struct_coordinates coords in ((MIDITrackProvider)MIDIAP).ActiveNotesLazyList)
            {
                (float a, int b) = calcHeightAndNoteFromIndex(coords);
                float rectY = a;
                float rectX = coords.x * TI.timeUnitSquareWidth;

                canvas.FillColor = Color.FromArgb("#bc4338");
                canvas.FillRectangle(rectX, rectY, TI.timeUnitSquareWidth, KI.pianoRollLinesHeight[b]);
            }

            (float h, int _i) = calcHeightAndNoteFromIndex(hoveredNote);
            float hoveredRectY = h;
            float hoveredRectX = hoveredNote.x * TI.timeUnitSquareWidth;

            canvas.FillColor = Color.FromArgb("#55bc4338");
            canvas.FillRectangle(hoveredRectX, hoveredRectY, TI.timeUnitSquareWidth, KI.pianoRollLinesHeight[_i]);
        }

        public void ToggleNote(Global.struct_coordinates noteCoords)
        {
            if(noteCoords.x >= 0 && noteCoords.y >= 0)
            {
                if (((MIDITrackProvider)MIDIAP).ActiveNotes[noteCoords.x, noteCoords.y] > 0)
                {
                    ((MIDITrackProvider)MIDIAP).ActiveNotes[noteCoords.x, noteCoords.y] = 0;
                    ((MIDITrackProvider)MIDIAP).ActiveNotesLazyList.Remove(noteCoords);
                }
                else
                {
                    ((MIDITrackProvider)MIDIAP).ActiveNotes[noteCoords.x, noteCoords.y] = 1;
                    ((MIDITrackProvider)MIDIAP).ActiveNotesLazyList.Add(noteCoords);
                }
            }
        }

        public void HoverNote(Global.struct_coordinates noteCoords)
        {
            if (hoveredNote.x != noteCoords.x || hoveredNote.y != noteCoords.y)
            {
                hoveredNote = noteCoords;
            }
        }

        private (float, int) calcHeightAndNoteFromIndex(Global.struct_coordinates coords)
        {
            float c = 0;
            int i = 0, j = 0;

            while (j < coords.y)
            {
                c += KI.pianoRollLinesHeight[i];
                i++;
                j++;

                if (i == 12) i = 0;
            }

            return (c, i);
        }
    }
}