using System.Net;
using System.Text.RegularExpressions;
using Octokit;

namespace github2trello.GitHub;

public static class GitHubApi
{
    private static readonly GitHubClient Client = new(new ProductHeaderValue("github2trello"));
    private static readonly string ClientId = EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
    private static readonly string ClientSecret = EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");
    private static readonly Regex CommentsRegex = new("<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

    public static async Task Login()
    {
        var loginRequest = new OauthLoginRequest(ClientId);

        var url = Client.Oauth.GetGitHubLoginUrl(loginRequest);
        Console.WriteLine($"Open {url} to authorize the application");

        using var listener = new HttpListener();

        listener.Prefixes.Add("http://localhost:58292/");
        listener.Start();

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString.Get("code");

        var tokenRequest = new OauthTokenRequest(ClientId, ClientSecret, code);
        var token = await Client.Oauth.CreateAccessToken(tokenRequest);

        Client.Credentials = new Credentials(token.AccessToken);
    }
    
    public static async Task<Dictionary<string, List<Issue>>> PRsMerged(
        string repoOwner,
        List<(string repoName, bool requireChangelog)> repoNames,
        DateTimeOffset createdAfter,
        DateTimeOffset createdBefore)
    {
        var pullRequests = new Dictionary<string, List<Issue>>();

        foreach (var (repoName, requireChangelog) in repoNames)
        {
            Console.WriteLine($"Searching merged prs PRs for {repoOwner}/{repoName} between {createdAfter} and {createdBefore}");
            var itemsFound = -1;
            pullRequests.Add(repoName, new List<Issue>());

            for (var page = 1; itemsFound != 0; page++)
            {
                var searchRequest = new SearchIssuesRequest
                {
                    Is = new[] {IssueIsQualifier.PullRequest, IssueIsQualifier.Merged},
                    Merged = DateRange.Between(createdAfter, createdBefore),
                    Page = page,
                    PerPage = 100
                };
                searchRequest.Repos.Add(repoOwner, repoName);

                var search = await Client.Search.SearchIssues(searchRequest);

                itemsFound = search.Items.Count;

                var items = search.Items;
                if (requireChangelog)
                {
                    items = search.Items.Where(item => HasChangelog(item.Body)).ToList();
                    Console.WriteLine($"Skipping {itemsFound - items.Count}/{itemsFound} PRs without a changelog");
                }
            
                pullRequests[repoName].AddRange(items);
            }
        }
        
        Console.WriteLine($"Found {pullRequests.Values.Sum(prs => prs.Count)} PRs");
        return pullRequests;
    }

    public static async Task<List<Author>> Contributors(string repoOwner, IEnumerable<string> repoNames, DateTimeOffset after, DateTimeOffset before)
    {
        var contributors = new Dictionary<int, Author>();

        foreach (var repoName in repoNames)
        {
            Console.WriteLine($"Searching contributors for {repoOwner}/{repoName} between {after} and {before}");
            var commits = await Client.Repository.Commit.GetAll(repoOwner, repoName, new CommitRequest
            {
                Since = after,
                Until = before
            });

            foreach (var commit in commits)
            {
                if (commit.Author == null)
                {
                    continue;
                }

                contributors.TryAdd(commit.Author.Id, commit.Author);
            }
        }

        var contributorList = contributors.Values.ToList();
        contributorList.Sort((a, b) => string.Compare(a.Login, b.Login, StringComparison.OrdinalIgnoreCase));
        return contributorList;
    }

    private static bool HasChangelog(string? body)
    {
        return body != null && CommentsRegex.Replace(body, string.Empty).Contains(":cl:");
    }
}