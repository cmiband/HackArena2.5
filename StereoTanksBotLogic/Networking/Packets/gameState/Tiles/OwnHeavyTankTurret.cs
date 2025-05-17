using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents turret of an player own heavy tank.
/// </summary>
/// <param name="Direction">Represents turret direction.</param>
/// <param name="BulletCount">Represents number of available bullets.</param>
/// <param name="TicksToBullet">Represents time in ticks to regenerate bullet.</param>
/// <param name="TicksToLaser">Represents time in ticks to regenerate laser.</param>
/// <param name="ticksToHealingBullet">Represents time in ticks to regenerate healing bullet.</param>
/// <param name="ticksToStunBullet">Represents time in ticks to regenerate stun bullet.</param>
public record class OwnHeavyTankTurret(Direction Direction, int BulletCount, int? TicksToBullet, int? TicksToLaser, int? TicksToHealingBullet, int? TicksToStunBullet);
