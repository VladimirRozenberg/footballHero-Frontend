using CommunityToolkit.Mvvm.Input;

namespace footballHero.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using footballHero.Services;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;


public class SlotInfo
{
    public string SlotId { get; set; } = string.Empty; 
    public string Role { get; set; } = string.Empty;   
}

public partial class PlayerSlot : ObservableObject
{
    
    [ObservableProperty] private int _playerId;
    
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _attackScore;

    [ObservableProperty]
    private int _defenseScore;
    [ObservableProperty]
    private int _value;
    
    public string DisplayValue => $"{Value / 1_000_000.0:0.#} mil";
    
    

}
public class GridPlayerDto
{
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("attack")]
    public int Attack { get; set; }

    [JsonPropertyName("defense")]
    public int Defense { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public class RandomPlayerDto
{
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }
    
    [JsonPropertyName("player_name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("player_position")]
    public string Position { get; set; } = string.Empty;
    
    [JsonPropertyName("player_attack")]
    public int Attack { get; set; }
    
    [JsonPropertyName("player_defense")]
    public int Defense { get; set; }
    
    [JsonPropertyName("player_value")]
    public string Value { get; set; } =string.Empty;
    
    [JsonPropertyName("active_status")]
    public string ActiveStatus { get; set; } = string.Empty;
}



public class InitializeSquadResponse
{
    [JsonPropertyName("grid")]
    public Dictionary<string, GridPlayerDto?> Grid { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("totalAttack")]
    public int TotalAttack { get; set; }

    [JsonPropertyName("totalDefense")]
    public int TotalDefense { get; set; }

    [JsonPropertyName("budget")]
    public int Budget { get; set; }

    [JsonPropertyName("completed")]
    public bool Completed { get; set; }
}



public partial class DraftPageViewModel : ViewModelBase
{
    [ObservableProperty] private string? _clubName;
    [ObservableProperty] private string? _overallAttack;
    [ObservableProperty] private string? _overallDefense;
    [ObservableProperty] private string? _budget;
    [ObservableProperty] bool _continueButtonVisible = false;
    [ObservableProperty]
    private PlayerSlot? _p1;
    [ObservableProperty]
    private PlayerSlot? _p2;
    [ObservableProperty]
    private PlayerSlot? _p3;
    [ObservableProperty]
    private PlayerSlot? _p4;
    [ObservableProperty]
    private PlayerSlot? _p5;
    [ObservableProperty] private string? _errorMessage =string.Empty;
    
    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty] 
    private ObservableCollection<RandomPlayerDto> _candidatePlayers = new ObservableCollection<RandomPlayerDto>();
    [ObservableProperty]
    private RandomPlayerDto? _selectedCandidate;
    
    public class AddPlayerRequest
    {
        [JsonPropertyName("clubId")]
        public int ClubId { get; set; }

        [JsonPropertyName("playerId")]
        public int PlayerId { get; set; }

        [JsonPropertyName("gridPosition")]
        public string GridPosition { get; set; } = string.Empty;
    }
    
    partial void OnSelectedCandidateChanged(RandomPlayerDto? value)
    {
        if (value == null) return;
        _= AddPlayer(value);
    }
    public DraftPageViewModel()
    {
        _ = InitializeDraft();
    }

    private async Task InitializeDraft()
    {
        var clubId = Session.Instance.Draft?.ClubId;
        if (clubId == null) return;

        var connector = new ConnectToFastApi();
        var result = await connector.GetAsync<InitializeSquadResponse>($"draft/initialize_squad?club_id={clubId}");
        if (result == null) return;

        var draft = Session.Instance.Draft!;
        draft.TotalAttack = result.TotalAttack;
        draft.TotalDefense = result.TotalDefense;
        draft.Total = result.Total;
        draft.Completed = result.Completed;
        draft.TransferBudget = result.Budget;

        ApplyGridToViewModel(result.Grid);
    }

    private async Task GetRandomPlayers(string position)
    {
        try
        {
            CandidatePlayers.Clear();
            var clubId = Session.Instance.Draft?.ClubId;
            if (clubId == null) {
                ErrorMessage = "No active club session found.";
                IsErrorOpen = true;
                return;
            }
            var connector = new ConnectToFastApi();
            var result = await connector.GetAsync<List<RandomPlayerDto>>($"draft/get_random_players?position={position}&club_id={clubId}");
            if (result == null) {
                ErrorMessage = "No players found.";
                IsErrorOpen = true;
                return;
            }
            
            
            foreach (var dto in result)
            {
                CandidatePlayers.Add(dto);
            }

        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            IsErrorOpen = true;
        }
        
    }
    
    private void ApplyGridToViewModel(Dictionary<string, GridPlayerDto?> grid)
    {
        P1 = ToSlot(grid.GetValueOrDefault("P1"));
        P2 = ToSlot(grid.GetValueOrDefault("P2"));
        P3 = ToSlot(grid.GetValueOrDefault("P3"));
        P4 = ToSlot(grid.GetValueOrDefault("P4"));
        P5 = ToSlot(grid.GetValueOrDefault("P5"));
        
        bool allFilled = P1 != null && P2 != null && P3 != null && P4 != null && P5 != null;
        ContinueButtonVisible = allFilled && !(Session.Instance.Draft?.Completed ?? false);

        OverallAttack = $"Attack: {Session.Instance.Draft?.TotalAttack ?? 0}";
        OverallDefense = $"Defense: {Session.Instance.Draft?.TotalDefense ?? 0}";
        Budget = $"Budget: {Session.Instance.Draft?.TransferBudget ?? 0}";
        ClubName = Session.Instance.Draft?.ClubName;
    }
    
    private static PlayerSlot? ToSlot(GridPlayerDto? dto)
    {
        if (dto == null) return null;
        return new PlayerSlot
        {
            PlayerId = dto.PlayerId,
            Name = dto.Name,
            AttackScore = dto.Attack,
            DefenseScore = dto.Defense,
            Value = dto.Value
        };
    }
    private PlayerSlot? GetSlotById(string slotId) => slotId switch
    {
        "P1" => P1,
        "P2" => P2,
        "P3" => P3,
        "P4" => P4,
        "P5" => P5,
        _ => null
    };

[RelayCommand]
    private Task Return()
    {
        Session.Instance.ClearDraft();
        AppServices.Navigation.NavigateTo(new MainMenuViewModel());
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CompleteDraft()
    {
        var clubId = Session.Instance.Draft?.ClubId;
        if (clubId == null) return;

        var connector = new ConnectToFastApi();
        var request = new AddPlayerRequest
        {
            ClubId = clubId.Value,
            PlayerId = 0,
            GridPosition = string.Empty
        };

        var result = await connector.SecurePostAsync<AddPlayerRequest, InitializeSquadResponse>("draft/complete_draft", request);
        if (result == null) return;

        var draft = Session.Instance.Draft!;
        draft.TotalAttack = result.TotalAttack;
        draft.TotalDefense = result.TotalDefense;
        draft.Total = result.Total;
        draft.Completed = result.Completed;
        draft.TransferBudget = result.Budget;

        ApplyGridToViewModel(result.Grid);
        
        await Task.Delay(600);
        Session.Instance.ClearDraft();
        AppServices.Navigation.NavigateTo(new LeaderboardPageViewModel());
    }
    
    private string _pendingSlotId = string.Empty;
    
    [RelayCommand]
    private void SelectPlayer(SlotInfo info)
    {
        string slot = info.SlotId; 
        string role = info.Role;
        _pendingSlotId = slot;
        _=GetRandomPlayers(role);

    }
    
    [RelayCommand]
    private async Task DeletePlayer(SlotInfo info)
    {
        var slot = GetSlotById(info.SlotId);   // <-- this line was missing
        string role = info.Role;
        if (info == null)
            return;
        
        if (slot == null)
        {
            ErrorMessage = $"Could not find player slot '{info.SlotId}'.";
            IsErrorOpen = true;
            return;
        }
        await DeletePlayer(info.SlotId, slot.PlayerId);
    }


    private async Task DeletePlayer(string slot, int playerId)
    {
        try
        {
            var clubId = Session.Instance.Draft?.ClubId;
            if (clubId == null) 
                throw new Exception("No active club session found.");
        
            var connector = new ConnectToFastApi();
            var request = new AddPlayerRequest
            {
                ClubId = clubId.Value,
                PlayerId = playerId,
                GridPosition = slot
            };
        
            var result = await connector.SecurePostAsync<AddPlayerRequest, InitializeSquadResponse>("draft/delete_player", request);
            if (result == null) 
                throw new Exception("Failed to delete player from backend.");
        
            var draft = Session.Instance.Draft!;
            draft.TotalAttack = result.TotalAttack;
            draft.TotalDefense = result.TotalDefense;
            draft.Total = result.Total;
            draft.Completed = result.Completed;
            draft.TransferBudget = result.Budget;
            ApplyGridToViewModel(result.Grid);
        }
        catch (Exception ex)
        {
            // Update the text in your Flyout and show it over the Trigger button
            ErrorMessage = ex.Message;            
            IsErrorOpen = true;
            
        }
    }
    private async Task AddPlayer(RandomPlayerDto candidate)
    {
        try
        {
            Console.WriteLine("🔥 ADD PLAYER STARTED");
            var clubId = Session.Instance.Draft?.ClubId;
            if (clubId == null) throw new Exception("No active club session found.");
            
            var connector = new ConnectToFastApi();
            var request = new AddPlayerRequest
            {
                ClubId = clubId.Value,
                PlayerId = candidate.PlayerId,
                GridPosition = _pendingSlotId
            };
            
            var result = await connector.SecurePostAsync<AddPlayerRequest, InitializeSquadResponse>("draft/add_player", request);
            if (result == null) throw new Exception("Failed to add player.");
            var draft = Session.Instance.Draft!;
            draft.TotalAttack = result.TotalAttack;
            draft.TotalDefense = result.TotalDefense;
            draft.Total = result.Total;
            draft.Completed = result.Completed;
            draft.TransferBudget = result.Budget;

            ApplyGridToViewModel(result.Grid);

            CandidatePlayers.Clear();
            SelectedCandidate = null;

        }
        catch (Exception e)
        {
            Console.WriteLine("🔥 ADD PLAYER CATCH RAN");
            Console.WriteLine(e.Message);
            ErrorMessage = e.Message;
            IsErrorOpen = true;
        }
        
    }
    
}