using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace footballHero.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent(); // <-- IT GOES RIGHT HERE!
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}