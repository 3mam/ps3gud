
using System.Reflection;

class Args
{
  public string id { get; private set; } = default!;
  public string download { get; private set; } = default!;
  string? _version { get; set; } = default!;
  bool _showHelp = false;
  bool _showVersion = false;
  public static Args Get(string[] args)
  {
    var newArgs = args.Append("\n").Select(v => v.ToLower()).ToArray();
    return new Args()
    {
      _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
      id = FindArgValue(newArgs, new [] { "-i", "--id" }),
      download = FindArgValue(newArgs, new [] { "-d", "--download" }),
      _showHelp = FindArgForShowInfo(newArgs, new [] { "-h", "--help" }),
      _showVersion = FindArgForShowInfo(newArgs, new [] { "-v", "--version" }),
    };
  }

  public void CheckIfVariableDefine()
  {
    if (id == "")
    {
      Console.WriteLine("Game Id is not defined");
      Environment.Exit(0);
    }
  }

  public void ShowInfoIfEnable()
  {
    if (_showVersion)
    {
      Console.WriteLine($"{_version}");
      Environment.Exit(0);
    }
    else if (_showHelp)
    {
      Console.WriteLine(@"
PlayStation 3 Games Update Downloader

optional arguments:
  -h, --help            show this help message and exit
  -v                    show program's version number and exit
  -i ID, --id ID
                        games id
  -d DOWNLOAD, --download DOWNLOAD
                        download folder
");
      Environment.Exit(0);
    }

  }
  static bool FindArgForShowInfo(string[] args, string[] cmd)
  {
    foreach (var v in args)
    {
      if (cmd.Contains(v))
        return true;
    }
    return false;

  }
  static string FindArgValue(string[] args, string[] cmd)
  {
    var newArgs = args.Select((item, index) => (item, index));
    foreach (var (v, i) in newArgs)
    {
      if (v == "\n")
        return "";
      var next = args[i + 1];
      var value = !cmd.Contains(next)
      ? next == "\n" ? "" : next
      : "";
      if (cmd.Contains(v))
        return value;
    }
    return "";
  }
}