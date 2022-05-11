namespace github2trello.Trello.API;

public static class TrelloApi
{
    public const string ApiUrl = "https://api.trello.com/1/";
    public static readonly HttpClient Client = new();
    public static readonly string Key = EnvExtensions.GetOrThrow("API_KEY");
    public static readonly string Token = EnvExtensions.GetOrThrow("API_TOKEN");
}
