using github2trello;
using github2trello.GitHub;
using github2trello.Trello.API;
using Octokit;

EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");
EnvExtensions.GetOrThrow("TRELLO_API_KEY");
EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

Console.WriteLine("Input a repo owner. Example: space-wizards");
var repoOwner = Console.ReadLine() ?? throw new NullReferenceException();

Console.WriteLine("Input a repo name. Example: space-station-14");
var repoName = Console.ReadLine() ?? throw new NullReferenceException();

Console.WriteLine("Input a starting date (YYYY-MM-DD). Example: 2022-04-01");
var after = DateExtensions.FixMonthDays(Console.ReadLine());

Console.WriteLine("Input an ending date (YYYY-MM-DD). Example: 2022-04-30");
var before = DateExtensions.FixMonthDays(Console.ReadLine());

await GitHubApi.Login();

var contributors = await GitHubApi.Contributors(repoOwner, repoName, after, before);
var contributorNames = string.Join(',', contributors.Select(author => author.Login));
Console.WriteLine($"Contributors: {contributorNames}");

var prs = new List<Issue>(500);
prs.AddRange(await GitHubApi.PRsMerged(repoOwner, repoName, after, before));
var list = await TrelloLists.Create($"Imported PRs ({repoName})", args[0]);

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