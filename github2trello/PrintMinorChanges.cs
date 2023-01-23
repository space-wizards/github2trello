using System.Text.RegularExpressions;
using github2trello.GitHub;
using github2trello.Trello.API;
using Manatee.Trello;
using static System.StringComparison;

namespace github2trello;

public class PrintMinorChanges
{
    public static void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        // https://trello.com/app-key
        var trelloApiKey = EnvExtensions.GetOrThrow("TRELLO_API_KEY");
        var trelloApiToken = EnvExtensions.GetOrThrow("TRELLO_API_TOKEN");

        TrelloAuthorization.Default.AppKey = trelloApiKey;
        TrelloAuthorization.Default.UserToken = trelloApiToken;

        Console.WriteLine("Paste the link to the Trello board:");
        var boardUrl = Console.ReadLine() ?? throw new NullReferenceException();

        Console.WriteLine($"Getting board id for url {boardUrl}");
        var boardId = (await TrelloBoards.UrlToBoardId(boardUrl)).Id;

        Console.WriteLine($"Searching lists in board with id {boardId}");
        var lists = await TrelloBoards.GetLists(boardId);
        Console.WriteLine($"Found {lists.Length} lists");

        var minorChanges = lists.First(board => board.Name.Equals("minor changes", InvariantCultureIgnoreCase));
        var cards = await TrelloLists.GetCards(minorChanges.Id);
        Console.WriteLine($"Found {cards.Length} minor changes cards");

        var lines = new List<string>();
        foreach (var card in cards)
        {
            var trelloCard = new Card(card.Id);
            await trelloCard.Comments.Refresh();
            if (trelloCard.Comments.FirstOrDefault() is not { } comment)
                continue;

            foreach (var changelog in GitHubApi.GetChangelog(comment.Data.Text))
            {
                lines.Add($"{changelog} {card.Desc.Split("\n")[2]}");
            }
        }

        Console.WriteLine($"Found {lines.Count} lines");

        foreach (var line in lines)
        {
            // TODO fix the console scrunklying itself here
            Console.WriteLine(line);
        }
    }
}
