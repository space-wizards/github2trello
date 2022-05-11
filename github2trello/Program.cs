using github2trello.GitHub;
using github2trello.Trello.API;
using Octokit;

await GitHubApi.Login();

const string createdAfter = "2022-04-01";
const string createdBefore = "2022-04-30";

var prs = new List<Issue>(500);
prs.AddRange(await GitHubSearch.PRsMerged("space-wizards", "space-station-14", createdAfter, createdBefore));
var list = await TrelloLists.Create("Imported PRs (Space Station 14)", args[0]);

var cards = new List<Func<ValueTask>>();
foreach (var pr in prs)
{
    var name = $"{pr.Title} (#{pr.Number})";
    var desc = $"{pr.HtmlUrl}\n\n*Contributed by {pr.User.Login}*";
    
    cards.Add(async () =>
    {
        await TrelloCards.Create(list.Id, name, desc);
    });
}

Console.WriteLine($"Creating {cards.Count} cards");
await Parallel.ForEachAsync(cards, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (func, _) => func());