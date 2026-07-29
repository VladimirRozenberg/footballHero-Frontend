using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using footballHero.Services;
using footballHero.ViewModels;

namespace footballHero.ViewModels;

public partial class LoginPageViewModel : ViewModelBase
{
    [ObservableProperty] private string? responseBox;
    [ObservableProperty] private string? passwordBox;
    [ObservableProperty] private string? usernameBox;
    
    public Action OnLoginSuccess { get; set; }
    
    
    [RelayCommand]
     private async Task TestConnection()
    {
        Console.WriteLine("TestConnection invoked");
        ResponseBox = "Connecting to footballhero.ch...";
        try
        {
            var connector = new ConnectToFastApi();

            var result = await connector.GetAsync<UserSession>("/"); 
            
            if (result != null)
            { 
                ResponseBox = $"🎉 Success!\nResponse from server:\n{result.UserName}";
            }
            else
            { 
                ResponseBox = "❌ Connected, but received a null response.";
            }
        }
        catch (Exception ex)
        {
            if (ResponseBox != null)
            {
                ResponseBox = $"❌ Network Error:\n{ex.Message}";
            }
        }
        
        
    }
     
     

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }    
    }
    
    private class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
    [RelayCommand]
    private async Task LogIn()
    {
        ResponseBox = "Connecting to footballhero.ch...";

      
        // 2. Extract safe, non-null strings from the text boxes
        string? username = UsernameBox;
        string? password = PasswordBox;

        // 3. Pack those safe strings into your request DTO 
        // (Using the variables we just made prevents NullReferenceExceptions)
        var loginData = new LoginRequest()
        {
            Username = username,
            Password = password
        };
        
        try
        {
            // 4. Fire up the connector
            var connector = new Services.ConnectToFastApi();
            // 5. Send it as a proper POST request with your type-strict data
             var result = await connector.PostAsync<LoginResponse, LoginRequest>("auth/login", loginData);
            
            ResponseBox = $"Login Successful!\nServer Response: {result.Message} {result.Token}";
            TokenStorage.SaveToken(result.Token);
            Console.WriteLine(TokenStorage.LoadToken());
            bool isVerified = await TokenVerification.VerifyToken();
            if (isVerified)
            {
                // Safe access using ? just in case User is null
                string sessionName = Session.Instance.User?.UserName; 
                Console.WriteLine($"TEST PASSED: Token is valid. Session hydrated for: {sessionName}");
                ResponseBox += $"\nTest Passed! Welcome to the session, {sessionName}";
                await Task.Delay(2000);
                OnLoginSuccess?.Invoke();
                
                
            }
            else
            {
                Console.WriteLine("TEST FAILED: Token verification rejected it.");
            }
            
            
            
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        
            if (ResponseBox != null)
            {
                ResponseBox = $"Login Failed:\n{exception.Message}";
            }
        }
    }
    [RelayCommand]
    private async Task SignIn(){
        ResponseBox = "Connecting to footballhero.ch...";
        
        
        string? username = UsernameBox;
        string? password = PasswordBox;

        var loginData = new LoginRequest()
        {
            Username = username,
            Password = password
        };
        if (ResponseBox != null){
            ResponseBox= "Connecting to footballhero.ch...";}

        try
        {
            var connector = new ConnectToFastApi();
            var result = await connector.PostAsync<LoginResponse, LoginRequest>("auth/user_create", loginData);
            if (result != null)
            {
                ResponseBox = $"Sign Up Successful!\nServer Response: {result.Message} {result.Token}";
                TokenStorage.SaveToken(result.Token);
                bool isVerified = await TokenVerification.VerifyToken();
                if (isVerified)
                {
                    string sessionName = Session.Instance.User?.UserName;
                    Console.WriteLine($"TEST PASSED: Token is valid. Session hydrated for: {sessionName}");
                    ResponseBox += $"\nTest Passed! Welcome to the session, {sessionName}";
                    await Task.Delay(2000);
                    OnLoginSuccess?.Invoke();

                    
                }

            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            if (ResponseBox != null)
            {
                ResponseBox = $"Login Failed:\n{exception.Message}";
            }
        }
    }
    
}


