namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents game end player.
/// </summary>
/// <param name="Name">
/// Represents name of a player.
/// </param>
/// <param name="Color">
/// Represents color of a player.
/// </param>
/// <param name="Score">
/// Represents score of a player.
/// </param>
/// <param name="players">
/// Represents players in a team.
/// </param>
public record class GameEndTeam(string Name, uint Color, int Score, List<GameEndPlayer> Players);
