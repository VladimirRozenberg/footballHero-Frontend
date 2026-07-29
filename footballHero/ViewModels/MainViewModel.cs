using CommunityToolkit.Mvvm.ComponentModel;
using footballHero.Services;
using System.Threading.Tasks;
namespace footballHero.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial string Greeting { get; set; } = "Welcome to Avalonia!";
    
    [ObservableProperty] private object _currentPage;

    public MainViewModel()
    {
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
                GoToMainMenu();
                return;
            }
        }

        GotoLogin();
    }
    
    
    
    
    private void GotoLogin()
    {
        var loginViewModel = new LoginPageViewModel();
        loginViewModel.OnLoginSuccess = GoToMainMenu;
        CurrentPage = loginViewModel;
    }
    
    private void GoToMainMenu()
    {
        var mainViewModel= new MainMenuViewModel();
        mainViewModel.OnReturn = GotoLogin;
        CurrentPage = mainViewModel; 
    }
}