namespace web_api.Contracts.User.Responses;

public class IsUserRegisteredResponse(bool isRegistered)
{
    public bool IsRegistered { get; set; } = isRegistered;
}