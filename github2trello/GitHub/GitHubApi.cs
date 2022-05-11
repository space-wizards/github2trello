using System.Net;
using Octokit;

namespace github2trello.GitHub;

public static class GitHubApi
{
    public static readonly GitHubClient Client = new(new ProductHeaderValue("github2trello"));
    private static readonly string ClientId = EnvExtensions.GetOrThrow("GITHUB_CLIENT_ID");
    private static readonly string ClientSecret = EnvExtensions.GetOrThrow("GITHUB_CLIENT_SECRET");

    public static async Task Login()
    {
        var loginRequest = new OauthLoginRequest(ClientId);

        var url = Client.Oauth.GetGitHubLoginUrl(loginRequest);
        Console.WriteLine($"Please open {url} to authorize the application");
        
        var listener = new HttpListener();
        
        listener.Prefixes.Add("http://localhost:58292/");
        listener.Start();

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString.Get("code");

        var tokenRequest = new OauthTokenRequest(ClientId, ClientSecret, code);
        var token = await Client.Oauth.CreateAccessToken(tokenRequest);

        Client.Credentials = new Credentials(token.AccessToken);
    }
}