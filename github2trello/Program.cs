﻿using github2trello;
using github2trello.GitHub;
using github2trello.Trello.API;
using Manatee.Trello;

// https://github.com/settings/applications > Authorized OAuth Apps
EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");

// https://trello.com/app-key
var trelloApiKey = EnvExtensions.GetOrThrow("TRELLO_API_KEY");
var trelloApiToken = EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

TrelloAuthorization.Default.AppKey = Environment.GetEnvironmentVariable(trelloApiKey);
TrelloAuthorization.Default.UserToken = Environment.GetEnvironmentVariable(trelloApiToken);

Console.WriteLine("Paste the link to the Trello board:");
var trelloBoardUrl = Console.ReadLine() ?? throw new NullReferenceException();

Console.WriteLine($"Getting board id for url {trelloBoardUrl}");
var trelloBoard = (await TrelloBoards.UrlToBoardId(trelloBoardUrl)).Id;

const string repoOwner = "space-wizards";
var repos = new List<(string repoName, bool requireChangelog)>
{
    ("space-station-14", true),
    ("RobustToolbox", false)
};
var repoNames = repos.Select(repo => repo.repoName).ToList();

Console.WriteLine("Input a starting date (YYYY-MM-DD). Example: 2022-04-01");
var after = DateExtensions.FixMonthDays(Console.ReadLine());

Console.WriteLine("Input an ending date (YYYY-MM-DD). Example: 2022-04-30");
var before = DateExtensions.FixMonthDays(Console.ReadLine()).AddDays(1).AddMilliseconds(-1);

await GitHubApi.Login();

var contributors = await GitHubApi.Contributors(repoOwner, repoNames, after, before);
var contributorNames = string.Join(", ", contributors.Select(author => author.Login));
Console.WriteLine($"Contributors: {contributorNames}");

var prs = await GitHubApi.PRsMerged(repoOwner, repos, after, before);

string GetListName(string repoName)
{
    return $"Imported PRs ({repoName})";
}

var lists = await TrelloLists.Create(repoNames.Select(GetListName).ToList(), trelloBoard);

var cards = new List<Func<ValueTask>>();
foreach (var (repoName, repoPrs) in prs)
{
    var list = lists[GetListName(repoName)];

    foreach (var pr in repoPrs)
    {
        var name = $"{pr.Title} (#{pr.Number})";
        var desc = $"{pr.HtmlUrl}\n\n*Contributed by {pr.User.Login}*";
    
        cards.Add(async () =>
        {
            var card = await TrelloCards.Create(list.Id, name, desc);

            if (pr.Body is not { } prBody)
                return;

            await new Card(card.Id).Comments.Add($"#PR DESCRIPTION:\n\n{prBody}");
        });
    }
}

Console.WriteLine($"Creating {cards.Count} cards");
await Parallel.ForEachAsync(cards, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (func, _) => func());