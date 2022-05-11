using System.Text.Json;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloLists
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}lists";
    private static readonly JsonSerializerOptions _options = new() {PropertyNameCaseInsensitive = true};

    // ReSharper disable once ClassNeverInstantiated.Global
    public record CreateResponse(string Id, string Name, bool Closed, string IdBoard, int Pos);

    public static async Task<CreateResponse> Create(string name, string idBoard)
    {
        Console.WriteLine($"Creating card list with name {name} in board {idBoard}");
        var res = await Client.PostAsync($"{ApiUrl}?name={name}&idBoard={idBoard}&key={Key}&token={Token}", null);
        return await JsonSerializer.DeserializeAsync<CreateResponse>(await res.Content.ReadAsStreamAsync(), _options);
    }
}