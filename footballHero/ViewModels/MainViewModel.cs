using CommunityToolkit.Mvvm.ComponentModel;
using footballHero.Services;
using System.Threading.Tasks;
namespace footballHero.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial string Greeting { get; set; } = "Welcome to Avalonia!";
    
    [ObservableProperty] private object _currentPage;
    
    public NavigationService Nav => AppServices.Navigation;
    
     public MainViewModel()
    {
        Nav.NavigateTo(new LoginPageViewModel());
        _ = InitializeAsync();
    }
    
    private async Task InitializeAsync()
    {
        var token = TokenStorage.LoadToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            bool isVerified = await TokenVerification.VerifyToken();
            if (isVerified)
            {
                Nav.NavigateTo(new MainMenuViewModel());
                return;
            }
        }

        Nav.NavigateTo(new LoginPageViewModel());
    }
    
}
