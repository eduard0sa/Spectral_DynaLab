using System.Text.Json;
using static SDLab_GUI.Global;

namespace SDLab_GUI.Tutorials
{
    struct struct_tutorialData
    {
        public List<struct_tutorialStep> steps { get; set; }
    }

    struct struct_tutorialStep
    {
        public List<string> captions { get; set; }
        public List<string> objectsToHighlight { get; set; }
    }

    public partial class TutorialModalOverlayView : ContentPage
    {
        private struct_tutorialData tutorialData;
        MainPage editorMainPageRefOBJ;

        public TutorialModalOverlayView(MainPage _mainPageRefOBJ)
        {
            InitializeComponent();

            editorMainPageRefOBJ = _mainPageRefOBJ;

            Loaded += async delegate {
                await loadTutorialDataJSON();
                displayStep(0);
            };
        }

        private struct_coordinates getAbsoluteElementPosition(VisualElement e)
        {
            struct_coordinates absoluteElementPosition = new struct_coordinates() {
                x = (int)e.X,
                y = (int)e.Y
            };

            Element parentElement = e.Parent;

            while(parentElement is VisualElement parentVisElement)
            {
                absoluteElementPosition.x += (int)parentVisElement.X;
                absoluteElementPosition.y += (int)parentVisElement.Y;
                parentElement = parentElement.Parent;
            }

            return absoluteElementPosition;
        }

        private async Task loadTutorialDataJSON()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TutorialData/EditorTutorialData.json");
            using var reader = new StreamReader(stream);

            string contents = reader.ReadToEnd();

            tutorialData = JsonSerializer.Deserialize<struct_tutorialData>(contents);
        }

        private void displayStep(int stepIndex)
        {
            if(stepIndex < tutorialData.steps.Count)
            {
                for (int i = 0; i < tutorialData.steps[stepIndex].captions.Count; i++)
                {
                    Label captionLabel = new Label()
                    {
                        Text = tutorialData.steps[stepIndex].captions[i],
                        FontFamily = "Orbitron",
                        FontSize = (i == 0) ? 35 : 25,
                        TextColor = Color.FromArgb("#f0f0f0"),
                        Margin = new Thickness(0, 0, 0, 20),
                    };

                    TutorialContentLayout.Children.Add(captionLabel);
                }

                tutorialMainGrid.GestureRecognizers.Clear();

                if (stepIndex == 0)
                {
                    Button startTutorialBTN = new Button()
                    {
                        Text = tutorialData.steps[stepIndex].objectsToHighlight[0],
                        FontFamily = "Orbitron",
                        FontSize = 18,
                        BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"],
                        CornerRadius = 5,
                        HeightRequest = 50,
                        WidthRequest = 150,
                        Margin = new Thickness(0, 0, 0, 20)
                    };

                    startTutorialBTN.Clicked += delegate
                    {
                        TutorialContentLayout.Children.Clear();
                        displayStep(1);
                    };

                    TutorialContentLayout.Children.Add(startTutorialBTN);
                }
                else
                {
                    for (int i = 0; i < tutorialData.steps[stepIndex].objectsToHighlight.Count; i++)
                    {
                        int currIndex = (int)i;

                        struct_elementBoundInfo targetButtonRectBounds = editorMainPageRefOBJ.highlightBTN(tutorialData.steps[stepIndex].objectsToHighlight[i]);
                        struct_coordinates absoluteHighlightBoxPosition = getAbsoluteElementPosition((VisualElement)targetButtonRectBounds.sourceElement);

                        BoxView highlightBox = new BoxView()
                        {
                            BackgroundColor = Color.FromArgb("#20ffffff"),
                            CornerRadius = 5,
                            InputTransparent = true,
                            WidthRequest = targetButtonRectBounds.Bounds.Width,
                            HeightRequest = targetButtonRectBounds.Bounds.Height,
                            TranslationX = absoluteHighlightBoxPosition.x,
                            TranslationY = absoluteHighlightBoxPosition.y
                        };

                        TutorialOpenAbsoluteSpace.Children.Add(highlightBox);

                        TapGestureRecognizer buttonAreaClickGestureRecognizer = new TapGestureRecognizer();

                        buttonAreaClickGestureRecognizer.Tapped += (o, e) =>
                        {
                            var position = e.GetPosition(targetButtonRectBounds.sourceElement);

                            if (position != null)
                            {
                                double x = position.Value.X;
                                double y = position.Value.Y;

                                if (x >= 0 && x <= targetButtonRectBounds.Bounds.Width && y >= 0 && y <= targetButtonRectBounds.Bounds.Height)
                                {
                                    editorMainPageRefOBJ.MIDITrackBTNClickCommand.Execute(null);

                                    TutorialContentLayout.Children.Clear();
                                    TutorialOpenAbsoluteSpace.Children.Clear();
                                    editorMainPageRefOBJ.stopBTNHighlight(tutorialData.steps[stepIndex].objectsToHighlight[currIndex]);

                                    displayStep(stepIndex + 1);
                                }
                            }
                        };

                        tutorialMainGrid.GestureRecognizers.Add(buttonAreaClickGestureRecognizer);
                    }
                }
            }
            else
            {
                editorMainPageRefOBJ.closeTutorialOverlay();
                return;
            }
        }
    }
}