using System.Text.Json;
class GamesDatabase
{
  GameData[]? _db = default!;
  public static GamesDatabase Load(string fileName)
  {
    var file = File.ReadAllText(fileName);
    var json = JsonSerializer.Deserialize<GameData[]>(file);
    return new GamesDatabase()
    {
      _db = json
    };
  }

  public GameData[] FindById(string id)
  {
    var items = _db?.Where(v => v.id == id.ToUpper()).ToArray() ?? default!;
    if (items.Length == 0)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"{id} not found!");
      Environment.Exit(-1);
    }
    return items;
  }

}