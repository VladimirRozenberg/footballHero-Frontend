using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace footballHero.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent(); // The source generator will handle this completely!
    }

    // This handles the button click event from your MainView.axaml
    private async void ConnectButton_Click(object? sender, RoutedEventArgs e)
    {
        // Find the TextBox named 'ResponseTextBox' in your layout
        var responseTextBox = this.FindControl<TextBox>("ResponseBox");
        
        if (responseTextBox != null)
        {
            responseTextBox.Text = "Connecting to footballhero.ch...";
        }
        
        try
        {
            // Instantiate your connection logic from the root namespace
            var connector = new footballHero.Connect();

            // Fire the request to your public root endpoint
            var result = await connector.GetAsync<dynamic>("/"); 

            if (responseTextBox != null)
            {
                if (result.ValueKind != System.Text.Json.JsonValueKind.Null && 
                    result.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    responseTextBox.Text = $"🎉 Success!\nResponse from server:\n{result}";
                }
                else
                {
                    responseTextBox.Text = "❌ Connected, but received a null response.";
                }
            }
        }
        catch (Exception ex)
        {
            if (responseTextBox != null)
            {
                responseTextBox.Text = $"❌ Network Error:\n{ex.Message}";
            }
        }
    }
}