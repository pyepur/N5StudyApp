using Microsoft.Maui.Controls;

namespace N5StudyApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    // This is the .NET 9 way to launch the app!
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Tell the master window to load our Traffic Cop (AppShell) instead of a specific page
        return new Window(new AppShell());
    }
}