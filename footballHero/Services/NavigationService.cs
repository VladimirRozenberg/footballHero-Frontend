using CommunityToolkit.Mvvm.ComponentModel;

namespace footballHero.Services;

public partial class NavigationService : ObservableObject
{
    [ObservableProperty] 
        private object? _currentPage;
    
        // Navigate to any ViewModel instance
        public void NavigateTo(object viewModel)
        {
            CurrentPage = viewModel;
        }
        
        
}

public static class AppServices
{
    public static NavigationService Navigation { get; } = new NavigationService();
}