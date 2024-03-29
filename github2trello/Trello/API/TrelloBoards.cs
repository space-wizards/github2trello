﻿using System.Text.Json;
using static System.StringComparison;
using static github2trello.Trello.API.TrelloApi;

namespace github2trello.Trello.API;

public static class TrelloBoards
{
    private const string ApiUrl = $"{TrelloApi.ApiUrl}boards";

    public record BoardResponse(string Id);

    public static async Task<BoardResponse> UrlToBoardId(string url)
    {
        var httpRes = await Client.GetAsync($"{url}.json?key={Key}&token={Token}");
        return await JsonSerializer.DeserializeAsync<BoardResponse>(await httpRes.Content.ReadAsStreamAsync(), Options) ?? throw new NullReferenceException();
    }

    public static async Task<TrelloLists.ListResponse[]> GetLists(string boardId)
    {
        var httpRes = await Client.GetAsync($"{ApiUrl}/{boardId}/lists?key={Key}&token={Token}");
        return await JsonSerializer.DeserializeAsync<TrelloLists.ListResponse[]>(await httpRes.Content.ReadAsStreamAsync(), Options) ?? throw new NullReferenceException();
    }
}