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
        var boardId = EnvExtensions.GetOrThrow("TRELLO_BOARD");
        Console.WriteLine($"Searching lists in board with id {boardId}");
        
        var lists = await TrelloBoards.GetLists(boardId);
        Console.WriteLine($"Found {lists.Length} lists");
        
        var minorChanges = lists.First(board => board.Name.Equals("minor changes", InvariantCultureIgnoreCase));
        var cards = await TrelloLists.GetCards(minorChanges.Id);
        Console.WriteLine($"Found {cards.Length} minor changes cards");

        var lines = cards.SelectMany(card => card.Desc.Split("\n")).Where(line => line.StartsWith('-'));
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}