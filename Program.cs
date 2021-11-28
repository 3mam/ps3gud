
var db = GamesDatabase.Load("db.json");
var arguments = Args.Get(args);
arguments.ShowInfoIfEnable();
arguments.CheckIfVariableDefine();
if (arguments.download == "")
  Directory.CreateDirectory("download");
var id = arguments.id;
var downloadFolder = arguments.download == "" ? "download" : arguments.download;
var items = db.FindById(id);
var item = items.First();
Console.WriteLine($"{item.id} {item.name}");
foreach (var i in items)
{
  var foldersName = $"{downloadFolder}/{i.id} {i.name}/{i.version}/";
  var fileName = i.url.Split("/").Last();
  Directory.CreateDirectory(foldersName);
  await Fetch.To(i.url, $"{foldersName}/{fileName}");
  Console.WriteLine();
}