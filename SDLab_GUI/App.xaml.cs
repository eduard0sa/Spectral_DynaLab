#if WINDOWS
using Microsoft.UI.Windowing;
using WinRT.Interop;
#endif

namespace SDLab_GUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new Window(new AppShell());
            #if WINDOWS
            window.Created += (_, __) =>
            {
                var nativeWindow = (Microsoft.UI.Xaml.Window)window.Handler.PlatformView;
                var hwnd = WindowNative.GetWindowHandle(nativeWindow);
                var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(id);

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.Maximize();
                }
            };
            #endif

            return window;
        }
    }
}