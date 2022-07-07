using github2trello.Trello.API;
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
        var filteredLines = new List<string>();
        var cardsWithoutFilteredLines = new List<string>();
        const char startingWith = '-';
        foreach (var card in cards)
        {
            var cardLines = card.Desc.Split('\n');
            lines.AddRange(cardLines);

            var cardFilteredLines = cardLines.Where(line => line.StartsWith(startingWith)).ToList();
            filteredLines.AddRange(cardFilteredLines);

            if (cardFilteredLines.Count == 0)
            {
                cardsWithoutFilteredLines.Add(card.Name);
            }
        }

        Console.WriteLine($"Found {lines.Count} lines");

        if (cardsWithoutFilteredLines.Count > 0)
        {
            Console.WriteLine($"Found {cardsWithoutFilteredLines.Count} cards with no lines starting with {startingWith}:\n\n{string.Join('\n', cardsWithoutFilteredLines)}");
        }

        Console.WriteLine($"Found {filteredLines.Count} lines starting with {startingWith}:\n");

        foreach (var line in filteredLines)
        {
            Console.WriteLine(line);
        }
    }
}