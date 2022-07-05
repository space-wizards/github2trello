using github2trello;
using github2trello.GitHub;
using github2trello.Trello.API;

// https://github.com/settings/applications > Authorized OAuth Apps
EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");

// https://trello.com/app-key
EnvExtensions.GetOrThrow("TRELLO_API_KEY");
EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

var trelloBoard = EnvExtensions.GetOrThrow("TRELLO_BOARD");

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
            await TrelloCards.Create(list.Id, name, desc);
        });
    }
}

Console.WriteLine($"Creating {cards.Count} cards");
await Parallel.ForEachAsync(cards, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (func, _) => func());