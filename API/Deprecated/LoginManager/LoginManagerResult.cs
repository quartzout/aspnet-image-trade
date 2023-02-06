namespace Identity;

public class LoginManagerResult
{
    public LoginManagerResult(string error)
    {
        Success = false;
        Error = error;
        userID = null;
    }

    public LoginManagerResult(int resultID)
    {
        userID = resultID;
        Success = true;
        Error = null;
    }

    public bool Success { get; set; }
    public string? Error { get; set; }
    public int? userID { get; set; }
}