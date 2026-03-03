using SDLab_GUI.AudioSystemsLogic;
using SDLab_GUI.AudioSystemsLogic.TrackAudioSystems;
using SDLab_GUI.UIComponents.Editors;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    public class MIDITrackItem : TrackItem
    {
        private MIDIItemSFXButtonArea openMIDIEditorButton;
        private structSwitchData repeatTrackModeSwitchData;
        private structSliderData tempoSliderData;
        private TrackItem templateAudioProvider; //UI Component for the MIDI Template Audio Provider (Oscillator/File Track)

        public MIDITrackItem(AudioEngineMGMT audioManager, MainPage mainPage)
        {
            openMIDIEditorButton = new MIDIItemSFXButtonArea(this);
            openMIDIEditorButton.OpenSFXEditorEvent = openMIDIEditorEvent;

            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "MIDI TRACK", new List<string> { "GreenTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));

            trackAudioProvider = audioManager.LaunchAudioEngine(true);
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;

            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, trackAudioProvider.pushOSCVisSampleArray, trackAudioProvider);

            gainSliderData = new structSliderData()
            {
                minVal = 0.0f,
                maxVal = 0.5f,
                defVal = 0.5f,
                numDisplayDecPlaces = 2
            };

            repeatTrackModeSwitchData = new structSwitchData()
            {
                defValIndex = false
            };

            tempoSliderData = new structSliderData()
            {
                minVal = 0.2f,
                maxVal = 5f,
                defVal = 1.0f,
                numDisplayDecPlaces = 2
            };

            UIPaint();
        }

        protected override void UIPaint()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromArgb("#14141d");
            Margin = new Thickness(0, 10, 0, 0);

            //Adding Slider Controls to the main Control Group.
            TrackItemControls[0].addSliderControl("Gain:", enumBaseColor.GREEN, gainSliderData, gainChangeEvent);
            TrackItemControls[0].addSwitchControl("Repeat Track:", enumBaseColor.GREEN, repeatTrackModeSwitchData, repeatModeChangeEvent);
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            TrackItemControls[1].addSliderControl("BPM:", enumBaseColor.GREEN, tempoSliderData, tempoChangeEvent);

            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);
            Children.Add(TrackItemControls[0]);
            Children.Add(TrackItemControls[1]);
            Children.Add(openMIDIEditorButton);
            Children.Add(TrackItemWaveVizualizerArea);
        }

        private void gainChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newGain = (float)e.NewValue;
            ((MIDITrackProvider)trackAudioProvider).changeGain(newGain);
        }

        private void repeatModeChangeEvent(object? sender, ToggledEventArgs e)
        {
            bool newRepeatState = e.Value;
            ((MIDITrackProvider)trackAudioProvider).ChangeMIDITrackRepeatingMode(newRepeatState);
        }

        private void tempoChangeEvent(object? sender, ValueChangedEventArgs e)
        {
            float newTempo = (float)e.NewValue;
            ((MIDITrackProvider)trackAudioProvider).ChangeMIDITrackTempo(newTempo);
        }

        private void openMIDIEditorEvent(object? sender, EventArgs e)
        {
            if (audioEngineMGMT.output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                mainPageOBJ.PlayPauseExternalWrapper();
            }

            MIDIInterfaceEditor MIDIEditor = new MIDIInterfaceEditor(audioEngineMGMT, this, templateAudioProvider);
            mainPageOBJ.Navigation.PushModalAsync(MIDIEditor);

            MIDIEditor.ModalBoxCloseEvent = closeMIDIEditorEvent;
        }

        private void closeMIDIEditorEvent(object? sender, EventArgs e)
        {
            mainPageOBJ.Navigation.PopModalAsync();
        }

        public TrackItem SetMIDITrackTemplateAP(enumEngineType templateAPType)
        {
            switch (templateAPType)
            {
                case enumEngineType.Oscillator:
                    templateAudioProvider = new OscillatorItem(audioEngineMGMT, mainPageOBJ, false, false);
                    break;
            }

            return templateAudioProvider;
        }

        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);
        }
    }

    internal class MIDIItemSFXButtonArea : FlexLayout
    {
        private Button openSFXBTN = new Button();
        private EventHandler openSFXEditorEvent = delegate { };

        /// <summary>
        /// This property holds the function that runs when the user clicks on the Open SFX Button.
        /// </summary>
        public EventHandler OpenSFXEditorEvent
        {
            get
            {
                return openSFXEditorEvent;
            }
            set
            {
                openSFXBTN.Clicked -= openSFXEditorEvent;
                openSFXEditorEvent = value;
                openSFXBTN.Clicked += openSFXEditorEvent;
            }
        }

        /// <summary>
        /// OscillatorItemSFXButtonArea class constructor.
        /// </summary>
        /// <param name="parentFLNode">Reference to the parent FlexLayout object (the Oscillator Item UI Component).</param>
        public MIDIItemSFXButtonArea(FlexLayout parentFLNode)
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            VerticalOptions = LayoutOptions.Fill;
            parentFLNode.SetGrow(this, 0.12f);

            openSFXBTN.Text = "Abrir MIDI";
            openSFXBTN.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            openSFXBTN.FontAttributes = FontAttributes.Bold;
            openSFXBTN.FontFamily = "Orbitron";
            openSFXBTN.CornerRadius = 0;
            openSFXBTN.HeightRequest = 100;
            openSFXBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            this.SetGrow(openSFXBTN, 1.0f);

            Children.Add(openSFXBTN);
        }
    }
}