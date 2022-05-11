using Octokit;
using static github2trello.GitHub.GitHubApi;

namespace github2trello.GitHub;

public static class GitHubSearch
{
    public static async Task<List<Issue>> PRsMerged(string repoOwner, string repoName, string createdAfter, string createdBefore)
    {
        Console.WriteLine($"Searching merged prs PRs for {repoOwner}/{repoName} between {createdAfter} and {createdBefore}");
        var pullRequests = new List<Issue>();
        var itemsFound = -1;

        for (var page = 1; itemsFound != 0; page++)
        {
            
            var searchRequest = new SearchIssuesRequest
            {
                Is = new[] {IssueIsQualifier.PullRequest, IssueIsQualifier.Merged},
                Merged = DateRange.Between(DateTimeOffset.Parse(createdAfter), DateTimeOffset.Parse(createdBefore)),
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
}