using System.Net;
using Octokit;

namespace github2trello.GitHub;

public static class GitHubApi
{
    private static readonly GitHubClient Client = new(new ProductHeaderValue("github2trello"));
    private static readonly string ClientId = EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
    private static readonly string ClientSecret = EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");

    public static async Task Login()
    {
        var loginRequest = new OauthLoginRequest(ClientId);

        var url = Client.Oauth.GetGitHubLoginUrl(loginRequest);
        Console.WriteLine($"Open {url} to authorize the application");
        
        var listener = new HttpListener();
        
        listener.Prefixes.Add("http://localhost:58292/");
        listener.Start();

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString.Get("code");

        var tokenRequest = new OauthTokenRequest(ClientId, ClientSecret, code);
        var token = await Client.Oauth.CreateAccessToken(tokenRequest);

        Client.Credentials = new Credentials(token.AccessToken);
    }
    
    public static async Task<List<Issue>> PRsMerged(string repoOwner, string repoName, DateTimeOffset createdAfter, DateTimeOffset createdBefore)
    {
        Console.WriteLine($"Searching merged prs PRs for {repoOwner}/{repoName} between {createdAfter} and {createdBefore}");
        var pullRequests = new List<Issue>();
        var itemsFound = -1;

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
            pullRequests.AddRange(search.Items);
        }
        
        Console.WriteLine($"Found {pullRequests.Count} PRs");
        return pullRequests;
    }

    public static async Task<List<Author>> Contributors(string repoOwner, string repoName, DateTimeOffset after, DateTimeOffset before)
    {
        Console.WriteLine($"Searching contributors for {repoOwner}/{repoName} between {after} and {before}");
        var commits = await Client.Repository.Commit.GetAll(repoOwner, repoName, new CommitRequest
        {
            Since = after,
            Until = before
        });

        var contributors = new Dictionary<int, Author>();
        foreach (var commit in commits)
        {
            if (commit.Author == null)
            {
                continue;
            }

            contributors.TryAdd(commit.Author.Id, commit.Author);
        }

        var contributorList = contributors.Values.ToList();
        contributorList.Sort((a, b) => string.Compare(a.Login, b.Login, StringComparison.Ordinal));
        
        return contributorList;
    }
}