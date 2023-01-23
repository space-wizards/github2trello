using System.Text.Json;
using Manatee.Trello;

namespace github2trello.Trello.API;

public static class TrelloApi
{
    public const string ApiUrl = "https://api.trello.com/1/";
    public static readonly HttpClient Client = new();
    public static readonly JsonSerializerOptions Options = new() {PropertyNameCaseInsensitive = true};
    public static readonly string Key = EnvExtensions.GetOrThrow("TRELLO_API_KEY");
    public static readonly string Token = EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

    public static void Authorize()
    {
        // https://trello.com/app-key
        var trelloApiKey = EnvExtensions.GetOrThrow("TRELLO_API_KEY");
        var trelloApiToken = EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

        TrelloAuthorization.Default.AppKey = trelloApiKey;
        TrelloAuthorization.Default.UserToken = trelloApiToken;
    }
}
