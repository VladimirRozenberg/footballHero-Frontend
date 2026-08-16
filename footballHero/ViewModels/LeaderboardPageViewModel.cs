using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using footballHero.Services;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;

namespace footballHero.ViewModels;

public partial class LeaderboardPageViewModel : ViewModelBase 
{
    [ObservableProperty] private ObservableCollection<LeaderboardResponse> _leaderboard = new ObservableCollection<LeaderboardResponse>();

    public LeaderboardPageViewModel()
    {
        _ = InitializeLeaderboard();
    }
    
    private async Task InitializeLeaderboard()
    {
        var connector = new ConnectToFastApi();

        try
        {
            var savedLeaderboard = await connector.GetAsync<List<LeaderboardResponse>>("/save/load_leaderboard");
            if (savedLeaderboard != null && savedLeaderboard.Count > 0)
            {
                Leaderboard.Clear();
                foreach (var leaderboard in savedLeaderboard)
                {
                    Leaderboard.Add(leaderboard);
                }
                
                Console.WriteLine(savedLeaderboard);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    
    public class LeaderboardResponse
    {
        public int UserId { get; init; }
        public int Position { get; init; }
        public string ClubName { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public int TotalScore { get; init; }
        public int TotalAttack { get; init; }
        public int TotalDefense { get; init; }
        
        public bool IsCurrentUser { get; init; }
    }
    
    [RelayCommand]
    private Task Return()
    {
        AppServices.Navigation.NavigateTo(new MainMenuViewModel());
        return Task.CompletedTask;
    }

    
}
    
