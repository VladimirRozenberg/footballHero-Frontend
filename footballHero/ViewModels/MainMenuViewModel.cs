using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using footballHero.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace footballHero.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{

    [ObservableProperty] private string? responseBox;
    [ObservableProperty] private string? teamName;
    public string WelcomeMessage => $"Welcome {Session.Instance.User?.UserName ?? "Player"}!";
    
    [RelayCommand]
    private async Task LogOut()
    {
        Session.Instance.ClearSession();
        TokenStorage.DeleteToken();
        await Task.Delay(500);
        AppServices.Navigation.NavigateTo(new LoginPageViewModel());
        
    }
    [RelayCommand]
    private async Task GotoLoadPage()
    {
        await Task.Delay(500);
        AppServices.Navigation.NavigateTo(new LoadPageViewModel());
    }

    public class TeamNameInsert
    {
        public string TeamName { get; set; }
        
    }

    public class TeamNameInsertResponse
    {
        [JsonPropertyName("saveId")]
        public int SaveId { get; set; }

        [JsonPropertyName("clubId")]
        public int ClubId { get; set; }

        [JsonPropertyName("currentMatchday")]
        public int CurrentMatchDay { get; set; }

        [JsonPropertyName("clubName")]
        public string ClubName { get; set; } = string.Empty;

        [JsonPropertyName("transferBudget")] public decimal TransferBudget { get; set; }
    }
    
    
    [RelayCommand]
    private async Task CreateTeam()
    {
        if (string.IsNullOrWhiteSpace(TeamName))
        {
            ResponseBox = "Team name cannot be empty!";
            return;
        }
        
        var teamInsert = new TeamNameInsert(){TeamName = TeamName};


        try
        {
            string inspectJson = System.Text.Json.JsonSerializer.Serialize(teamInsert);
            Console.WriteLine($"[DEBUG OUTBOUND JSON]: {inspectJson}");
            var connctor = new ConnectToFastApi();
            var result = await connctor.SecurePostAsync<TeamNameInsert,TeamNameInsertResponse>("save/create_team", teamInsert);

            if (result != null)
            {
                Session.Instance.SetDraft(new DraftSession
                {
                    ClubId =  result.ClubId,
                    ClubName =  result.ClubName,
                    CurrentMatchDay =  result.CurrentMatchDay,
                    SaveId =  result.SaveId,
                    TransferBudget =  result.TransferBudget
                });
            }
            ResponseBox = result?.ClubName ?? "Something went wrong.";        
            
            await Task.Delay(3000);
            AppServices.Navigation.NavigateTo(new DraftPageViewModel());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}