using github2trello.Trello.API;

var list = await new TrelloLists().Create("Imported PRs", args[0]);
await new TrelloCards().Create(list.Id, "test2", "test3");