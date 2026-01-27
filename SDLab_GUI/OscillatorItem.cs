namespace SDLab_GUI
{
    class OscillatorItem : FlexLayout
    {
        FlexLayout OscillatorTriangleMark = new FlexLayout();
        FlexLayout OscillatorItemHeader = new FlexLayout();
        FlexLayout OscillatorItemSliderControls = new FlexLayout();

        OscillatorItem()
        {
            HorizontalOptions = LayoutOptions.Fill;
            Direction = Microsoft.Maui.Layouts.FlexDirection.Row;
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween;
            HeightRequest = 100;
            BackgroundColor = Color.FromHex("#14141d");

            //FlexLayout.Grow="0.05" JustifyContent="Center" AlignItems="Center"
            OscillatorTriangleMark.JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            OscillatorTriangleMark.AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;

            //FlexLayout.Grow="0.16" JustifyContent="Center" AlignItems="Center"
            OscillatorItemHeader.JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            OscillatorItemHeader.AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;

            //FlexLayout.Grow="0.16" Direction="Column" JustifyContent="Center" AlignItems="Center" VerticalOptions="Fill"
            OscillatorItemSliderControls.Direction = Microsoft.Maui.Layouts.FlexDirection.Column;
            OscillatorItemSliderControls.JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center;
            OscillatorItemSliderControls.AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center;
            OscillatorItemSliderControls.VerticalOptions = LayoutOptions.Fill;
        }
    }
}
