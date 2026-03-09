using System.Text.Json;
using Microsoft.Maui.Controls.Shapes;
using static SDLab_GUI.Global;

namespace SDLab_GUI.Tutorials
{
    /// <summary>
    /// Stores tutorial data from the retrieved JSON file.
    /// </summary>
    struct struct_tutorialData
    {
        public List<struct_tutorialStep> steps { get; set; }
    }

    /// <summary>
    /// Stores tutorial individual step data from the retrieved JSON file.
    /// </summary>
    struct struct_tutorialStep
    {
        public List<string> captions { get; set; }
        public List<string> objectsToHighlight { get; set; }
    }

    /// <summary>
    /// Represents the tutorial modal overlay view.
    /// </summary>
    public partial class TutorialModalOverlayView : ContentPage
    {
        private struct_tutorialData tutorialData;
        MainPage editorMainPageRefOBJ;

        /// <summary>
        /// TutorialModalOverlayView class constructor.
        /// </summary>
        /// <param name="_mainPageRefOBJ"></param>
        public TutorialModalOverlayView(MainPage _mainPageRefOBJ)
        {
            InitializeComponent();

            editorMainPageRefOBJ = _mainPageRefOBJ;

            Loaded += async delegate {
                await loadTutorialDataJSON();
                displayStep(0);
            };
        }

        /// <summary>
        /// This method calculates the absolute position of an element in the UI.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This asynchronous method loads the tutorial data from the JSON source file, deserializing it into the tutorialData struct.
        /// </summary>
        /// <returns>The async task data.</returns>
        private async Task loadTutorialDataJSON()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TutorialData/EditorTutorialData.json");
            using var reader = new StreamReader(stream);

            string contents = reader.ReadToEnd();

            tutorialData = JsonSerializer.Deserialize<struct_tutorialData>(contents);
        }

        /// <summary>
        /// This method displays the data of a specific tutorial step, based on the provided step index from the JSON-retrieved data.
        /// </summary>
        /// <param name="stepIndex">Step int index (beggins with 0).</param>
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

                    startTutorialBTN.Clicked += startTutorialBTNEvent;

                    TutorialContentLayout.Children.Add(startTutorialBTN);
                }
                else
                {
                    for (int i = 0; i < tutorialData.steps[stepIndex].objectsToHighlight.Count; i++)
                    {
                        int currIndex = (int)i;

                        struct_elementBoundInfo targetButtonRectBounds = editorMainPageRefOBJ.highlightBTN(tutorialData.steps[stepIndex].objectsToHighlight[i]);

                        drawButtonHighlighter(targetButtonRectBounds);

                        TapGestureRecognizer buttonAreaClickGestureRecognizer = new TapGestureRecognizer();

                        buttonAreaClickGestureRecognizer.Tapped += (o, e) =>
                        {
                            Point? position = e.GetPosition(targetButtonRectBounds.sourceElement);
                            Button targetButton = (Button)editorMainPageRefOBJ.FindByName(tutorialData.steps[stepIndex].objectsToHighlight[currIndex]);

                            if (position != null && targetButton != null)
                            {
                                double x = position.Value.X;
                                double y = position.Value.Y;

                                if (x >= 0 && x <= targetButtonRectBounds.Bounds.Width && y >= 0 && y <= targetButtonRectBounds.Bounds.Height)
                                {
                                    targetButton.Command.Execute(null);

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

        /// <summary>
        /// This method proceeds to the tutorial when the user clicks on the tutorial start button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startTutorialBTNEvent(object? sender, EventArgs e)
        {
            TutorialContentLayout.Children.Clear();
            displayStep(1);
        }

        /// <summary>
        /// This method draws a highlight element around the button to be clicked.
        /// </summary>
        /// <param name="targetButtonRectBounds"></param>
        private void drawButtonHighlighter(struct_elementBoundInfo targetButtonRectBounds)
        {
            struct_coordinates absoluteHighlightBoxPosition = getAbsoluteElementPosition((VisualElement)targetButtonRectBounds.sourceElement);

            Border highlightBorder = new Border()
            {
                BackgroundColor = Colors.Transparent,
                InputTransparent = true,
                WidthRequest = targetButtonRectBounds.Bounds.Width,
                HeightRequest = targetButtonRectBounds.Bounds.Height,
                TranslationX = absoluteHighlightBoxPosition.x,
                TranslationY = absoluteHighlightBoxPosition.y
            };
            highlightBorder.StrokeShape = new RoundRectangle()
            {
                CornerRadius = 5,
                BackgroundColor = Colors.White
            };
            highlightBorder.StrokeThickness = 5;

            TutorialOpenAbsoluteSpace.Children.Add(highlightBorder);
        }
    }
}