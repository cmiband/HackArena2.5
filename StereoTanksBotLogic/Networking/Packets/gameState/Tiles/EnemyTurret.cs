using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents turret of an enemy tank.
/// </summary>
/// <param name="Direction">Represents turret direction.</param>
public record class EnemyTurret(Direction Direction);
