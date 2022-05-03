using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public class TrelloCards
{
    public const string ApiUrl = $"{TrelloApi.ApiUrl}cards";
    
    public async Task Create(string idList, string name, string desc)
    {
        var res = await Client.PostAsync($"{ApiUrl}?idList={idList}&name={name}&desc={desc}&key={Key}&token={Token}", null);
        if (!res.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync($"Error creating card with name {name} and description {desc}:" +
                                               $"{res.StatusCode} {res.ReasonPhrase}");
        }
    }
}