using Microsoft.Maui.Controls.Shapes;
using SDLab_GUI.UIComponents.Editors;
using System.Text.Json;
using static SDLab_GUI.Global;

namespace SDLab_GUI.Tutorials
{
    /// <summary>
    /// Stores tutorial data from the retrieved JSON file.
    /// </summary>
    public struct struct_tutorialData
    {
        public List<struct_tutorialStep> steps { get; set; }
    }

    /// <summary>
    /// Stores tutorial individual step data from the retrieved JSON file.
    /// </summary>
    public struct struct_tutorialStep
    {
        public List<string> captions { get; set; }
        public List<struct_objectToHighlight> objectsToHighlight { get; set; }
    }

    public struct struct_objectToHighlight
    {
        public string objectName { get; set; }
        public string objectType { get; set; }
        public string objectSourceName { get; set; }
    }

    /// <summary>
    /// Represents the tutorial modal overlay view.
    /// </summary>
    public partial class TutorialModalOverlayView : ContentPage
    {
        private struct_tutorialData tutorialData;
        MainPage editorMainPageRefOBJ;
        private bool alreadyLoaded = false;

        /// <summary>
        /// TutorialModalOverlayView class constructor.
        /// </summary>
        /// <param name="_mainPageRefOBJ"></param>
        public TutorialModalOverlayView(MainPage _mainPageRefOBJ)
        {
            InitializeComponent();

            editorMainPageRefOBJ = _mainPageRefOBJ;

            Loaded += async delegate {
                if (!alreadyLoaded)
                {
                    await loadTutorialDataJSON();
                    displayStep(0);
                    alreadyLoaded = true;
                }
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
            if (stepIndex < tutorialData.steps.Count)
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
                        Text = tutorialData.steps[stepIndex].objectsToHighlight[0].objectName,
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
                    int countMidiHightlightObjects = 0;
                    for (int i = 0; i < tutorialData.steps[stepIndex].objectsToHighlight.Count; i++)
                    {
                        int currIndex = (int)i;

                        struct_elementBoundInfo targetButtonRectBounds = editorMainPageRefOBJ.highlightBTN(tutorialData.steps[stepIndex].objectsToHighlight[i]);

                        if (tutorialData.steps[stepIndex].objectsToHighlight[currIndex].objectType == "VisualElement")
                        {
                            drawButtonHighlighter(targetButtonRectBounds);

                            TapGestureRecognizer buttonAreaClickGestureRecognizer = new TapGestureRecognizer();

                            buttonAreaClickGestureRecognizer.Tapped += (o, e) =>
                            {
                                Page targetPage = null;

                                for (int j = 0; j < Shell.Current.Navigation.NavigationStack.Count; j++)
                                {
                                    if (Shell.Current.Navigation.NavigationStack[j] != null)
                                    {
                                        if (Shell.Current.Navigation.NavigationStack[j].AutomationId == tutorialData.steps[stepIndex].objectsToHighlight[currIndex].objectSourceName)
                                        {
                                            targetPage = Shell.Current.Navigation.NavigationStack[j];
                                        }
                                    }
                                }

                                if (targetPage == null)
                                {
                                    for (int j = 0; j < Shell.Current.Navigation.ModalStack.Count; j++)
                                    {
                                        if (Shell.Current.Navigation.ModalStack[j] != null)
                                        {
                                            if (Shell.Current.Navigation.ModalStack[j].AutomationId == tutorialData.steps[stepIndex].objectsToHighlight[currIndex].objectSourceName)
                                            {
                                                targetPage = Shell.Current.Navigation.ModalStack[j];
                                            }
                                        }
                                    }
                                }

                                if (targetPage != null)
                                {
                                    Point? position = e.GetPosition(targetButtonRectBounds.sourceElement);
                                    VisualElement targetButton = targetPage.GetVisualTreeDescendants().OfType<VisualElement>().FirstOrDefault(e => e.AutomationId == tutorialData.steps[stepIndex].objectsToHighlight[currIndex].objectName);

                                    if (position != null && targetButton != null)
                                    {
                                        double x = position.Value.X;
                                        double y = position.Value.Y;

                                        if (x >= 0 && x <= targetButtonRectBounds.Bounds.Width && y >= 0 && y <= targetButtonRectBounds.Bounds.Height)
                                        {
                                            editorMainPageRefOBJ.closeTutorialOverlay();
                                            if (targetButton is Button)
                                            {
                                                ((Button)targetButton).Command.Execute(null);
                                            }
                                            else if(targetButton is ImageButton)
                                            {
                                                ((ImageButton)targetButton).Command.Execute(null);
                                            }
                                            else if (targetButton is Switch)
                                            {
                                                ((Switch)targetButton).IsToggled = !((Switch)targetButton).IsToggled;
                                            }

                                            TutorialContentLayout.Children.Clear();
                                            TutorialOpenAbsoluteSpace.Children.Clear();
                                            editorMainPageRefOBJ.stopBTNHighlight(tutorialData.steps[stepIndex].objectsToHighlight[currIndex]);

                                            Microsoft.UI.Xaml.DispatcherTimer nextStepTimer = new Microsoft.UI.Xaml.DispatcherTimer();
                                            nextStepTimer.Interval = TimeSpan.FromMilliseconds(500);
                                            nextStepTimer.Tick += delegate
                                            {
                                                editorMainPageRefOBJ.showTutorialOverlay();
                                                displayStep(stepIndex + 1);
                                                nextStepTimer.Stop();
                                            };
                                            nextStepTimer.Start();
                                        }
                                    }
                                }
                            };

                            tutorialMainGrid.GestureRecognizers.Add(buttonAreaClickGestureRecognizer);
                        }
                        else
                        {
                            countMidiHightlightObjects++;
                        }
                    }

                    if(countMidiHightlightObjects > 0)
                    {
                        Button okBTN = new Button()
                        {
                            Text = "OK!",
                            FontFamily = "Orbitron",
                            FontSize = 18,
                            BackgroundColor = (Color)Application.Current.Resources["DefaultPastelRed"],
                            CornerRadius = 5,
                            HeightRequest = 50,
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 0, 20)
                        };

                        okBTN.Clicked += delegate {
                            editorMainPageRefOBJ.closeTutorialOverlay();
                        };

                        Page targetPage = null;

                        if (targetPage == null)
                        {
                            for (int j = 0; j < Shell.Current.Navigation.ModalStack.Count; j++)
                            {
                                if (Shell.Current.Navigation.ModalStack[j] != null)
                                {
                                    if (Shell.Current.Navigation.ModalStack[j].AutomationId == "MIDIInterfaceEditorModal")
                                    {
                                        targetPage = Shell.Current.Navigation.ModalStack[j];
                                    }
                                }
                            }
                        }

                        if(targetPage != null)
                        {
                            ((MIDIInterfaceEditor)targetPage).PianoRoll.EndNotesHighlightTutorialTaskAction = delegate
                            {
                                editorMainPageRefOBJ.showTutorialOverlay();

                                TutorialContentLayout.Children.Clear();
                                TutorialOpenAbsoluteSpace.Children.Clear();

                                displayStep(stepIndex + 1);
                            };
                        }

                        TutorialContentLayout.Children.Add(okBTN);
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
                TranslationY = absoluteHighlightBoxPosition.y,
                Stroke = Color.FromArgb("#fffb42"),
            };
            highlightBorder.StrokeShape = new RoundRectangle()
            {
                CornerRadius = 5,
            };
            highlightBorder.StrokeThickness = 5;

            TutorialOpenAbsoluteSpace.Children.Add(highlightBorder);
        }
    }
}