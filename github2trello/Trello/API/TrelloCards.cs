using System.Text;
using System.Text.Json;
using System.Web;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloCards
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}cards";

    // ReSharper disable once ClassNeverInstantiated.Global
    public record CardResponse(string Id, string Desc, string Name);
    
    public static async Task<CardResponse> Create(string idList, string name, string desc)
    {
        idList = HttpUtility.UrlEncode(idList);
        name = HttpUtility.UrlEncode(name);
        desc = HttpUtility.UrlEncode(desc);

        var res = await Client.PostAsync($"{ApiUrl}?idList={idList}&name={name}&desc={desc}&key={Key}&token={Token}", null);
        res.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<CardResponse>(await res.Content.ReadAsStreamAsync(), Options) ?? throw new NullReferenceException();
    }
}