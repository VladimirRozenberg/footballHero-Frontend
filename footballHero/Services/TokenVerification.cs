namespace footballHero.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using footballHero.Services;

public class TokenVerification
{
    public static async Task<bool> VerifyToken()
    {
        string savedToken = TokenStorage.LoadToken();
        bool tokenExists = !string.IsNullOrEmpty(savedToken);
        try
        {
            if (tokenExists)
            {
                Console.WriteLine($"Token loaded: {savedToken}");
                var requestData = new VerifyTokenRequest{Token = savedToken}; 
                var connector = new Services.ConnectToFastApi();
                var result = await connector.PostAsync<VerifyTokenResponse, VerifyTokenRequest>("auth/token_verification", requestData);
                if (result != null)
                {
                    var loggedInUser = new UserSession()
                    {
                        UserId = result.UserId,
                        UserName = result.UserName
                    };
                
                    Session.Instance.SetSession(loggedInUser);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed internally: {e.Message}");
            return false;
        }
        

        return false;
    }
}
public class VerifyTokenRequest
{
    public string Token { get; set; }
}
public class VerifyTokenResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}