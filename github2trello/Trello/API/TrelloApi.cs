namespace github2trello.Trello.API;

public class TrelloApi
{
    public const string ApiUrl = "https://api.trello.com/1/";
    public static readonly HttpClient Client = new();
    public static readonly string Key = EnvironmentExtensions.GetOrThrow("API_KEY");
    public static readonly string Token = EnvironmentExtensions.GetOrThrow("API_TOKEN");
}
