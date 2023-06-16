using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinUIEx;
using WinWrapper;
using WinWrapper.Input;
using Window = Microsoft.UI.Xaml.Window;

namespace NoteFlyout;
public sealed partial class FlyoutSimulationWindow : Window
{
    public FlyoutSimulationWindow()
    {
        InitializeComponent();
        SystemBackdrop = new MicaBackdrop();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBarDrag);
        IsInitialized = true;
        var display = Display.FromPoint(Cursor.Position).WorkingAreaBounds;
        this.MoveAndResize(display.Right - App.FlyoutWidth - 16, display.Bottom - (App.FlyoutHeight + 48) - 16, App.FlyoutWidth, App.FlyoutHeight + 48);
    }
    readonly bool IsInitialized = false;
    private void FlyoutOpenStatusCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        if (FlyoutOpenStatus.IsChecked ?? false)
        {
            FlyoutContent.OpenFlyout();
            FlyoutContent.Visibility = Visibility.Visible;
        } else
        {
            FlyoutContent.CloseFlyout();
            FlyoutContent.Visibility = Visibility.Collapsed;
        }
    }

    private void TopMostCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        this.SetIsAlwaysOnTop(TopMost.IsChecked ?? false);
    }
}
