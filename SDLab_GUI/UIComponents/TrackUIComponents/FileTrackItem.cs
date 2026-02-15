using SDLab_GUI.AudioSystemsLogic;
using static SDLab_GUI.Global;

namespace SDLab_GUI.UIComponents.TrackUIComponents
{
    internal class FileTrackItem : TrackItem
    {
        public FileTrackItem(AudioEngineMGMT audioManager, MainPage mainPage)
        {
            TrackTriangleMark = new TrackItemLeftIconMenu(this);
            TrackTriangleMark.ClickEventHandler = deleteTrackEvent;
            TrackItemHeader = new TrackItemHeader(this, "OSCILLATOR", new List<string> { "YellowTitle" });
            TrackItemControls = new List<TrackItemSliderControlGroup>();
            TrackItemControls.Add(new TrackItemSliderControlGroup(this));

            trackAudioProvider = audioManager.LaunchAudioEngine();
            audioEngineMGMT = audioManager;
            mainPageOBJ = mainPage;

            TrackItemWaveVizualizerArea = new TrackItemWaveVizualizerArea(this, trackAudioProvider.pushOSCVisSampleArray, trackAudioProvider);

            gainSliderData = new Global.structSliderData()
            {
                minVal = 0f,
                maxVal = 0.5f,
                defVal = trackAudioProvider.CurrentGain,
                numDisplayDecPlaces = 2
            };

            UIPaint();
        }

        protected override void UIPaint()
        {

        }

        protected override void deleteTrackEvent(object? sender, EventArgs e)
        {
            TrackItemWaveVizualizerArea.UpdateFrameTimer.Stop();
            audioEngineMGMT.removeAudioEngine(trackAudioProvider);
            (Parent as VerticalStackLayout).Children.Remove(this);
        }
    }
}