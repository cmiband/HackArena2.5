using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents game end player.
/// </summary>
/// <param name="Id">
/// Represents id of a player.
/// </param>
/// <param name="Kills">
/// Represents player kills.
/// </param>
/// <param name="TankType">
/// Represents tank type.
/// </param>
public record class GameEndPlayer(string Id, int Kills, TankType TankType);
