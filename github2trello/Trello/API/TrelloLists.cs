using System.Text.Json;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloLists
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}lists";

    // ReSharper disable once ClassNeverInstantiated.Global
    public record ListResponse(string Id, string Name, bool Closed, string IdBoard, int Pos);

    public static async Task<Dictionary<string, ListResponse>> Create(List<string> names, string idBoard)
    {
        Console.WriteLine($"Creating card list with name {string.Join(", ", names)} in board {idBoard}");

        var lists = new Dictionary<string, ListResponse>();
        
        foreach (var name in names)
        {
            var httpRes = await Client.PostAsync($"{ApiUrl}?name={name}&idBoard={idBoard}&key={Key}&token={Token}", null);
            var res = await JsonSerializer.DeserializeAsync<ListResponse>(await httpRes.Content.ReadAsStreamAsync(), Options);
            lists[name] = res ?? throw new NullReferenceException();
        }

        return lists;
    }

    public static async Task<TrelloCards.CardResponse[]> GetCards(string listId)
    {
        var httpRes = await Client.GetAsync($"{ApiUrl}/{listId}/cards?key={Key}&token={Token}");
        return await JsonSerializer.DeserializeAsync<TrelloCards.CardResponse[]>(
                   await httpRes.Content.ReadAsStreamAsync(), Options) ?? throw new NullReferenceException();
    }
}