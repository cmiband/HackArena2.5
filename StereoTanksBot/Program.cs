using CommandLine;
using CommandLine.Text;
using StereoTanksBot.CommandLine;
using StereoTanksBot.Networking;
using StereoTanksBotLogic.Enums;

string host = "localhost";
string port = "5000";
string teamName = string.Empty;
TankType tankType = TankType.Light;
string code = string.Empty;

var parser = new Parser(with =>
{
    with.CaseSensitive = false;
    with.HelpWriter = null;
});

var parserResult = parser.ParseArguments<CommandLineOptions>(args);

if (parserResult.Tag == ParserResultType.NotParsed)
{
    Console.WriteLine("[System] Invalid command line arguments. Here are available commands:\n");
    Console.WriteLine(HelpText.AutoBuild(parserResult, null, null));

    return;
}

_ = parserResult.WithParsed((opts) =>
{
    if (!string.IsNullOrEmpty(opts.Host))
    {
        host = opts.Host;
    }

    if (!string.IsNullOrEmpty(opts.Port))
    {
        port = opts.Port;
    }

    if (!string.IsNullOrEmpty(opts.TeamName))
    {
        teamName = opts.TeamName;
    }

    if (opts.TankType.HasValue)
    {
        tankType = (TankType)opts.TankType;
    }

    if (!string.IsNullOrEmpty(opts.Code))
    {
        code = opts.Code;
    }
});

_ = parserResult.WithNotParsed<CommandLineOptions>((err) =>
{
    // Handle errors (if any)
    foreach (var error in err)
    {
        Console.WriteLine(error);
    }
});

BotWebSocketClient client = new(host, port, teamName, tankType, code);

var currentDomain = System.AppDomain.CurrentDomain;
currentDomain.ProcessExit += async (s, e) =>
{
    if (client.IsConnected)
    {
        await client.CloseAsync();
    }
};

await client.ConnectAsync();
