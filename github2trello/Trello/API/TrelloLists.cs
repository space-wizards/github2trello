using System.Text.Json;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public class TrelloLists
{
    public const string ApiUrl = $"{TrelloApi.ApiUrl}lists";

    public record CreateResponse(string Id, string Name, bool Closed, string IdBoard, int Pos);

    public async Task<CreateResponse> Create(string name, string idBoard)
    {
        var res = await Client.PostAsync($"{ApiUrl}?name={name}&idBoard={idBoard}&key={Key}&token={Token}", null);
        return await JsonSerializer.DeserializeAsync<CreateResponse>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    }
}