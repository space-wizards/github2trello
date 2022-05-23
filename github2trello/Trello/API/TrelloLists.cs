using System.Text.Json;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloLists
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}lists";
    private static readonly JsonSerializerOptions Options = new() {PropertyNameCaseInsensitive = true};

    // ReSharper disable once ClassNeverInstantiated.Global
    public record CreateResponse(string Id, string Name, bool Closed, string IdBoard, int Pos);

    public static async Task<Dictionary<string, CreateResponse>> Create(List<string> names, string idBoard)
    {
        Console.WriteLine($"Creating card list with name {string.Join(", ", names)} in board {idBoard}");

        var lists = new Dictionary<string, CreateResponse>();
        
        foreach (var name in names)
        {
            var httpRes = await Client.PostAsync($"{ApiUrl}?name={name}&idBoard={idBoard}&key={Key}&token={Token}", null);
            var res = await JsonSerializer.DeserializeAsync<CreateResponse>(await httpRes.Content.ReadAsStreamAsync(), Options);
            lists[name] = res ?? throw new NullReferenceException();
        }

        return lists;
    }
}