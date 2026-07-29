using footballHero.Services;

namespace footballHero.Services
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }


public class Session
{
    private static Session? _instance;
    public static Session Instance => _instance ??= new Session();
    public UserSession? User { get; private set; }
    public bool IsLoggedin => User != null;
    
    public void SetSession(UserSession user)
    {
        User = user;
    }

    public void ClearSession()
    {
        User = null;
    }
    
}
}

