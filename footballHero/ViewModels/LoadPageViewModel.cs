using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using footballHero.Services;
using System.Linq;
using CommunityToolkit.Mvvm.Input;


namespace footballHero.ViewModels;

public partial class LoadPageViewModel : ViewModelBase
{
    [ObservableProperty] private string? _responseBox;
    [ObservableProperty] private ObservableCollection<LoadSavesResponse> _saves = new ObservableCollection<LoadSavesResponse>();
    [ObservableProperty] private LoadSavesResponse? _selectedSave;
    public LoadPageViewModel()
    {
        _=InitializeSaves();
    }

    partial void OnSelectedSaveChanged(LoadSavesResponse? value)
    {
        if (value == null) return;



        ResponseBox = $"Selected Club: {value.ClubName} (ID: {value.ClubId})";
        
        Session.Instance.SetDraft(new DraftSession
        {
            ClubId = value.ClubId,
            ClubName =  value.ClubName,
            CurrentMatchDay = value.CurrentMatchDay,
            SaveId = value.SaveId,
            TransferBudget = value.TransferBudget
        });
        
        Console.WriteLine(Session.Instance.Draft.ClubName);
        AppServices.Navigation.NavigateTo(new DraftPageViewModel());

        
        SelectedSave = null;
    }
    
    public class LoadSavesResponse
    {
        public int SaveId { get; set; }
        
        public int ClubId { get; set; }
        public int CurrentMatchDay { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public decimal TransferBudget { get; set; }
        
    }

    private async Task InitializeSaves()
    {
        var connector = new ConnectToFastApi();
        
        try
        {
            var savesList = await connector.GetAsync<List<LoadSavesResponse>>("save/load_saves");
            if (savesList != null && savesList.Count > 0)
            {
                Saves.Clear();

                foreach (var saves in savesList)
                {
                    Saves.Add(saves);
                }
                
                var firstSave = savesList.FirstOrDefault();
                
                ResponseBox = $"🎉 Success!\nLoaded {savesList.Count} saves.\nFirst Save ID: {firstSave.SaveId}";
            }
            else
            {
                ResponseBox = "❌ Connected, but no save slots were found.";
            }
            
        }
        catch (Exception e)
        {
            ResponseBox = $"💥 Error connecting to backend:\n{e.Message}";
        }
        
        
    }

    [RelayCommand]
    private Task Return()
    {
        AppServices.Navigation.NavigateTo(new MainMenuViewModel());
        return Task.CompletedTask;
    }
    
    

}