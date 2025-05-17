namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents game end.
/// </summary>
/// <param name="Teams">
/// Represents list of teams stats at the end of the game.
/// </param>
public record class GameEnd(GameEndTeam[] Teams);
