namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents lobby data.
/// </summary>
/// <param name="PlayerId">Represents your own player id.</param>
/// <param name="TeamName">Represents your own team name.</param>
/// <param name="Teams">Represents teams list.</param>
/// <param name="ServerSettings">Represents current game server settings.</param>
public record class LobbyData(string PlayerId, string TeamName, LobbyTeam[] Teams, ServerSettings ServerSettings);
