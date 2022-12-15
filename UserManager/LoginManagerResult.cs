namespace UserManager;

public class LoginManagerResult
{
    public LoginManagerResult(string error)
    {
        Success = false;
        Error = null;
        userID = null;
    }

    public LoginManagerResult(int resultID)
    {
        userID = resultID;
        Success = true;
        Error = null;
    }

    bool Success { get; set; }
    string? Error { get; set; }
    int? userID { get; set; }
}