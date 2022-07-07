using System.Text;
using System.Web;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloCards
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}cards";

    // ReSharper disable once ClassNeverInstantiated.Global
    public record CardResponse(string Id, string Desc, string Name);
    
    public static async Task Create(string idList, string name, string desc)
    {
        idList = HttpUtility.UrlEncode(idList);
        name = HttpUtility.UrlEncode(name);
        desc = HttpUtility.UrlEncode(desc);

        var res = await Client.PostAsync($"{ApiUrl}?idList={idList}&name={name}&desc={desc}&key={Key}&token={Token}", null);
        if (!res.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync($"Error creating card with name {HttpUtility.UrlDecode(name)} and description {HttpUtility.UrlDecode(desc)}:" +
                                               $"{res.StatusCode} {res.ReasonPhrase}");
        }
    }
}