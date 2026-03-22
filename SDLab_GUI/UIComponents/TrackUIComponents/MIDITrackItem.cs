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
        private bool isTutorial;

        internal MIDIItemSFXButtonArea OpenMIDIEditorButton { get => openMIDIEditorButton; }

        public MIDITrackItem(AudioEngineMGMT audioManager, MainPage mainPage, bool _isTutorial = false, string _openMIDIBTNAutomationName = "")
        {
            openMIDIEditorButton = new MIDIItemSFXButtonArea(this);
            openMIDIEditorButton.OpenSFXEditorEvent = new Command(() => { openMIDIEditorEvent(new object(), new EventArgs()); });
            OpenMIDIEditorButton.OpenSFXBTN.AutomationId = _openMIDIBTNAutomationName;

            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "MIDI TRACK", new List<string> { "GreenTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));

            trackAudioProvider = audioManager.LaunchAudioEngine(isMIDI: true);
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;
            isTutorial = _isTutorial;

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
            TrackItemControls[0].addSliderControl("Ganho/Volume:", enumBaseColor.GREEN, gainSliderData, gainChangeEvent);

            string repeatTrackAutomationID = isTutorial ? $"repeatMIDI_{OpenMIDIEditorButton.OpenSFXBTN.AutomationId[OpenMIDIEditorButton.OpenSFXBTN.AutomationId.Length - 1]}" : null;
            TrackItemControls[0].addSwitchControl("Repetir faixa:", enumBaseColor.GREEN, repeatTrackModeSwitchData, repeatModeChangeEvent, repeatTrackAutomationID);
            /*TrackItemControls.Add(new TrackItemSliderControlGroup(this));
            TrackItemControls[1].addSliderControl("BPM:", enumBaseColor.GREEN, tempoSliderData, tempoChangeEvent);*/

            Children.Add(TrackTriangleMark);
            Children.Add(TrackItemHeader);
            Children.Add(TrackItemControls[0]);
            //Children.Add(TrackItemControls[1]);
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

            MIDIInterfaceEditor MIDIEditor = new MIDIInterfaceEditor(mainPageOBJ, audioEngineMGMT, this, templateAudioProvider);
            mainPageOBJ.Navigation.PushModalAsync(MIDIEditor);

            MIDIEditor.ModalBoxCloseEvent = closeMIDIEditorEvent;
        }

        private void closeMIDIEditorEvent()
        {
            ((MIDITrackProvider)trackAudioProvider).PauseMIDI();
            mainPageOBJ.Navigation.PopModalAsync();
        }

        public async Task<TrackItem> SetMIDITrackTemplateAP(enumEngineType templateAPType)
        {
            switch (templateAPType)
            {
                case enumEngineType.Oscillator:
                    templateAudioProvider = new OscillatorItem(audioEngineMGMT, mainPageOBJ, initialFrequency: 261.63f, addToMixer: false, _activateWaveGraphMonitor: false);
                    break;
                case enumEngineType.FileTrack:
                    var FileChoiceDialog = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecionar ficheiro de audio (.mp3, .wav, .ogg, etc...)."
                    });

                    if (FileChoiceDialog == null) return null;

                    templateAudioProvider = new FileTrackItem(audioEngineMGMT, mainPageOBJ, FileChoiceDialog.FullPath, false, false);
                    break;
            }

            return templateAudioProvider;
        }

        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);

            mainPageOBJ.checkEmptyTrackListMessageVisibility();
        }
    }

    internal class MIDIItemSFXButtonArea : FlexLayout
    {
        private Button openSFXBTN = new Button();
        private Command openSFXEditorEvent = new Command(() => { });

        /// <summary>
        /// This property holds the function that runs when the user clicks on the Open SFX Button.
        /// </summary>
        public Command OpenSFXEditorEvent
        {
            get
            {
                return openSFXEditorEvent;
            }
            set
            {
                openSFXEditorEvent = value;
                OpenSFXBTN.Command = openSFXEditorEvent;
            }
        }

        public Button OpenSFXBTN { get => openSFXBTN; }

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

            OpenSFXBTN.Text = "Abrir MIDI";
            OpenSFXBTN.FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
            OpenSFXBTN.FontAttributes = FontAttributes.Bold;
            OpenSFXBTN.FontFamily = "Orbitron";
            OpenSFXBTN.CornerRadius = 0;
            OpenSFXBTN.HeightRequest = 100;
            OpenSFXBTN.BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"];
            this.SetGrow(OpenSFXBTN, 1.0f);

            Children.Add(OpenSFXBTN);
        }
    }
}