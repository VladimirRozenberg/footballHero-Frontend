using System;
using System.Collections.Generic;
using footballHero.Services;
using System.Text.Json.Serialization;
namespace footballHero.Services
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    public class DraftSession
    {   
        public int SaveId { get; set; }
        public int ClubId { get; set; }
        public int CurrentMatchDay { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public decimal TransferBudget { get; set; }
        
        public int TotalAttack { get; set; } = 0;
        public int TotalDefense { get; set; } = 0;
        public int Total { get; set; } = 0;
        public bool Completed { get; set; } =false;
        public PlayerGrid Grid { get; set; } = new PlayerGrid();
    }

    public class PlayerSlot
    {
        [JsonPropertyName("player_id")] public int? PlayerId { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("attack")] public int Attack { get; set; } = 0;
        [JsonPropertyName("defense")] public int Defense { get; set; } = 0;
        [JsonPropertyName("value")] public int Value { get; set; } = 0;
        [JsonPropertyName("position")]public string Position { get; set; } = string.Empty;

    }

    public class PlayerGrid
    {
        public Dictionary<string, PlayerSlot> Players { get; set; } = new()
        {
            ["P1"] = new PlayerSlot(),
            ["P2"] = new PlayerSlot(),
            ["P3"] = new PlayerSlot(),
            ["P4"] = new PlayerSlot(),
            ["P5"] = new PlayerSlot(),

        };
    }


    public class Session
    {
        private static readonly Lazy<Session> _instance = new(() => new Session());
        public static Session Instance => _instance.Value;

        private Session() { }

        public UserSession? User { get; private set; }
        public DraftSession? Draft { get; private set; }

        public bool IsLoggedin => User != null;

        public void SetSession(UserSession user) => User = user;
        public void ClearSession() => User = null;

        public void SetDraft(DraftSession draft) => Draft = draft;
        public void ClearDraft() => Draft = null;
    }
    
    
}


