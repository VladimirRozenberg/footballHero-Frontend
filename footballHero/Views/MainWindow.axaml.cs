using Avalonia.Controls;
using System;
using System.Threading.Tasks;

namespace footballHero.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Fire the test method as soon as the window initializes
            // We use a discard '_' because we don't need to await the window constructor itself
            _ = RunTestConnectionAsync();
        }

        private async Task RunTestConnectionAsync()
        {
            Console.WriteLine("=== AVALONIA NETWORK TEST STARTING ===");
            
            // 1. Instantiate the connection module you just built
            var connector = new Connect();

            // 2. Call your public endpoint (swap "public-data" with your actual endpoint path, or "/" for root)
            // Replace 'YourExpectedRecord' with whatever object layout matches your JSON response
            var result = await connector.GetAsync<dynamic>(""); 

            // 3. Check your IDE debug output terminal!
            if (result != null)
            {
                Console.WriteLine("🎉 Success! Connected to FastAPI successfully.");
                Console.WriteLine($"Raw Response: {result}");
            }
            else
            {
                Console.WriteLine("❌ Connection failed. Look slightly further up in this terminal for the exception printout.");
            }
            
            Console.WriteLine("=====================================");
        }
    }
}