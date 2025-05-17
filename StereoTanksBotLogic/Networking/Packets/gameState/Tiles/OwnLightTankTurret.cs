using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents turret of an player own heavy tank.
/// </summary>
/// <param name="Direction">Represents turret direction.</param>
/// <param name="BulletCount">Represents number of available bullets.</param>
/// <param name="TicksToBullet">Represents time in ticks to regenerate bullet.</param>
/// <param name="TicksToDoubleBullet">Represents time in ticks to regenerate double bullet.</param>
public record class OwnLightTankTurret(Direction Direction, int BulletCount, int? TicksToBullet, int? TicksToDoubleBullet, int? TicksToHealingBullet, int? TicksToStunBullet);
