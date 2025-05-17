using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents team in lobby.
/// </summary>
/// <param name="Name">Represents team name.</param>
/// <param name="color">Represents team color.</param>
/// <param name="Players">Represents team players.</param>
public record class LobbyTeam(string Name, string Color, LobbyPlayer[] Players);
