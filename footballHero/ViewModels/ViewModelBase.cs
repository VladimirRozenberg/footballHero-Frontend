using CommunityToolkit.Mvvm.ComponentModel;

namespace footballHero.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
}

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial MainViewModel ViewModelBase { get; set; } = new MainViewModel();
}