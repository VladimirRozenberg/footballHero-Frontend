using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using footballHero.Services;
using footballHero.ViewModels;



namespace footballHero.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    public string WelcomeMessage => $"Welcome {Session.Instance.User?.UserName ?? "Player"}!";
    public Action OnReturn { get; set; }

    [RelayCommand]
    private Task LogOut()
    {
        Session.Instance.ClearSession();
        TokenStorage.DeleteToken();
        OnReturn?.Invoke();
        return Task.CompletedTask;
        
    }
}