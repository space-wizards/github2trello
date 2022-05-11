namespace github2trello.Trello.API;

public static class TrelloApi
{
    public const string ApiUrl = "https://api.trello.com/1/";
    public static readonly HttpClient Client = new();
    public static readonly string Key = EnvExtensions.GetOrThrow("TRELLO_API_KEY");
    public static readonly string Token = EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");
}
