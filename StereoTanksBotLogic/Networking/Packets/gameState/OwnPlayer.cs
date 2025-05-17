namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents your own player.
/// </summary>
/// <param name="Id">Represents your own players id.</param>
/// <param name="Nickname">Represents your own players nickname.</param>
/// <param name="Color">Represents your own players color.</param>
/// <param name="Ping">Represents your own players ping.</param>
/// <param name="Score">Represents your own players score.</param>
/// <param name="TicksToRegen">Represents your own players ticks to regenerate.</param>
/// <param name="IsUsingRadar">Represents radar usage.</param>
public record class OwnPlayer(
    string Id,
    int Ping,
    int? TicksToRegen)
    : GamePlayer(
        Id,
        Ping);
