using CommandLine;
using StereoTanksBotLogic.Enums;

namespace StereoTanksBot.CommandLine;

/// <summary>
/// Represents command line options.
/// </summary>
public class CommandLineOptions
{
    /// <summary>
    /// Gets or sets game server host.
    /// </summary>
    [Option('h', "host", Required = false, HelpText = "Set host.")]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets game server port.
    /// </summary>
    [Option('p', "port", Required = false, HelpText = "Set port.")]
    public string? Port { get; set; }

    /// <summary>
    /// Gets or sets player nickname.
    /// </summary>
    [Option('n', "team-name", Required = true, HelpText = "Set team name.")]
    public string? TeamName { get; set; }

    /// <summary>
    /// Gets or sets tank type.
    /// </summary>
    [Option('t', "tank-type", Required = true, HelpText = "Set tank type. Possible values: light, heavy")]
    public TankType? TankType { get; set; }

    /// <summary>
    /// Gets or sets game server join code.
    /// </summary>
    [Option('c', "code", Required = false, HelpText = "Set join code.")]
    public string? Code { get; set; }
}
