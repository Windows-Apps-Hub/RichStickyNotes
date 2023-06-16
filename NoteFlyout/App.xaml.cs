using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Get.OutOfBoundsFlyout;
using System.Diagnostics;
using WAH.NoteSystem.Core;
using Windows.ApplicationModel;
using WinWrapper;
using WAH.NoteSystem.UI.Controls;
using Application = Microsoft.UI.Xaml.Application;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;
using WinWrapper.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Get.WinUI.TransparentWindow.Helpers;
using System.Drawing;
using Icon = WinWrapper.Icon;

namespace NoteFlyout;

public partial class App : Application
{
    public const int FlyoutWidth = 600;
    public const int FlyoutHeight = 500;
    public App()
    {
        this.InitializeComponent();
    }
    readonly bool RealFlyoutMode = true;
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await NoteSystem.Current.EnsureSystemInitializedAsync();
        Debug.WriteLine(NoteSystem.Current.RootFolder.Path);
        if (RealFlyoutMode) SetupRealFlyout();
        else
        {
            new FlyoutSimulationWindow().Activate();
        }
        UnhandledException += (o, e) =>
        {
#if DEBUG
            Debugger.Break();
#endif
            e.Handled = true;
        };
    }
    void SetupRealFlyout()
    {
        WindowsSystemDispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
        OutOfBoundsFlyoutSystem.EnsureSystemActivated();
        NotifyIcon trayIcon = new()
        {
            Icon =
                Icon.FromHandle(
                    new Bitmap(@$"{Package.Current.InstalledPath}\Assets\LockScreenLogo.scale-200.png")
                    .GetHicon()
                ),
            Visible = true
        };

        var flyoutContent = new FlyoutContent()
        {
            Width = FlyoutWidth,
            Height = FlyoutHeight,
            CornerRadius = new(8),
            Margin = new(-16)
        };
        var flyoutPresenter = new FlyoutPresenter
        {
            Background = new AcrylicBrush
            {
                TintColor = Windows.UI.Color.FromArgb(255, 44, 44, 44),
                TintLuminosityOpacity = 0,// 0.96
            },
            Content = flyoutContent,
            MaxWidth = FlyoutWidth + 10,
            MaxHeight = FlyoutHeight + 10
        };
        //flyoutPresenter.Background = new SolidColorBrush(Colors.Transparent);
        var flyout = new CustomFlyout(flyoutPresenter);
        flyout.Opening += (_, _) => flyoutContent.OpenFlyout();
        flyout.Closing += (_, _) => flyoutContent.CloseFlyout();
        var ani = new DoubleAnimation()
        {
            From = 0,
            To = -500,
            Duration = new(TimeSpan.FromSeconds(2)),
            EnableDependentAnimation = true,
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut}
        };
        var ani2 = new DoubleAnimation()
        {
            From = 0,
            To = -500,
            Duration = new(TimeSpan.FromSeconds(2)),
            EnableDependentAnimation = true,
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        var storyboard = new Storyboard
        {
            Children =
            {
                ani, ani2
            }
        };
        TranslateTransform c;
        flyoutPresenter.RenderTransform = c = new TranslateTransform()
        {
            
        };
        Storyboard.SetTarget(ani, c);
        Storyboard.SetTargetProperty(ani, "X");
        Storyboard.SetTarget(ani2, c);
        Storyboard.SetTargetProperty(ani2, "Y");

        storyboard.AutoReverse = true;
        storyboard.RepeatBehavior = RepeatBehavior.Forever;
        trayIcon.LeftMouseButtonUp += async delegate
        {
            //if (e.MouseEvent is not MouseEvent.IconLeftMouseUp) return;
            OutOfBoundsFlyoutSystem.CloseFlyout();
            await Task.Delay(100);
            try
            {
                //storyboard.Begin();
                await OutOfBoundsFlyoutSystem.ShowAsync(
                    flyout,
                    Cursor.Position,
                    true,
                    FlyoutPlacementMode.Top
                );
            }
            catch
            {

            }
        };
    }
}
