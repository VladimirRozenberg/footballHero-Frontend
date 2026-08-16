using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using footballHero.ViewModels;
using footballHero.Views;
using footballHero.Services;

namespace footballHero;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 1. Give MainWindow the MainViewModel (The Router)
            // 2. DO NOT set the Content here! Let the XAML ContentControl do its job.
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            
            desktop.MainWindow.Width = 420;
            desktop.MainWindow.Height = 950;
            desktop.MainWindow.MinWidth = 400;
            desktop.MainWindow.MinHeight = 950;
            
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            // For mobile/browser
            singleViewFactoryApplicationLifetime.MainViewFactory =
                () => new MainView() { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // For mobile/browser
            singleViewPlatform.MainView = new MainView()
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public static class AppServices
    {
        public static NavigationService Navigation {  get; }  = new NavigationService();
    }
}