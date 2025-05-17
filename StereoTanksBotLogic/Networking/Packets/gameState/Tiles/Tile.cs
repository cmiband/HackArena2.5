using Newtonsoft.Json;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.JsonConverters;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents tile.
/// </summary>
/// <param name="ZoneIndex">Represents zone index in witch tile is located.</param>
/// <param name="Entities">Represents entities in a tile.</param>
[JsonConverter(typeof(TileJsonConverter))]
public record class Tile(int? ZoneIndex, Tile.TileEntity[] Entities)
{
    /// <summary>
    /// Represents base tile entity.
    /// </summary>
    public abstract record class TileEntity;

    /// <summary>
    /// Represents wall tile entity.
    /// </summary>
    public record class Wall(WallType Type) : TileEntity;

    /// <summary>
    /// Represents enemy tank tile entity.
    /// </summary>
    /// <param name="OwnerId">Represents owner id of an enemy tank.</param>
    /// <param name="Direction">Represents direction of an enemy tank.</param>
    /// <param name="Turret">Represents turret of an enemy tank.</param>
    /// <param name="IsUsingRadar">Represents radar usage.</param>
    public record class EnemyLightTank(string OwnerId, Direction Direction, EnemyTurret Turret, bool IsUsingRadar) : TileEntity;

    /// <summary>
    /// Represents enemy tank tile entity.
    /// </summary>
    /// <param name="OwnerId">Represents owner id of an enemy tank.</param>
    /// <param name="Direction">Represents direction of an enemy tank.</param>
    /// <param name="Turret">Represents turret of an enemy tank.</param>
    public record class EnemyHeavyTank(string OwnerId, Direction Direction, EnemyTurret Turret) : TileEntity;

    /// <summary>
    /// Represents own light tank tile entity.
    /// </summary>
    /// <param name="OwnerId">Represents owner id of player own tank.</param>
    /// <param name="Direction">Represents direction of player own tank.</param>
    /// <param name="Turret">Represents turret of player own light tank.</param>
    /// <param name="Health">Represents health of player own tank.</param>
    /// <param name="TicksToRadar">Represents time in ticks to regenerate radar.</param>
    /// <param name="IsUsingRadar">Represents radar usage.</param>
    [JsonConverter(typeof(OwnLightTankJsonConverter))]
    public record class OwnLightTank(string OwnerId, Direction Direction, OwnLightTankTurret Turret, int? Health, int? TicksToRadar, bool IsUsingRadar, bool[,] Visibility) : TileEntity;

    /// <summary>
    /// Represents own heavy tank tile entity.
    /// </summary>
    /// <param name="OwnerId">Represents owner id of player own tank.</param>
    /// <param name="Direction">Represents direction of player own tank.</param>
    /// <param name="Turret">Represents turret of player own heavy tank.</param>
    /// <param name="Health">Represents health of player own tank.</param>
    /// <param name="TicksToMine">Represents time in ticks to regenerate mine.</param>
    [JsonConverter(typeof(OwnHeavyTankJsonConverter))]
    public record class OwnHeavyTank(string OwnerId, Direction Direction, OwnHeavyTankTurret Turret, int? Health, int? TicksToMine, bool[,] Visibility) : TileEntity;

    /// <summary>
    /// Represents bullet tile entitiy.
    /// </summary>
    /// <param name="Direction">Represents bullet direction.</param>
    /// <param name="Id">Represents bullet id.</param>
    /// <param name="Speed">Represents bullet speed.</param>
    /// <param name="Type">Represents bullet type.</param>
    public record class Bullet(Direction Direction, int Id, double Speed, BulletType Type) : TileEntity;

    /// <summary>
    /// Represents laser tile entitiy.
    /// </summary>
    /// <param name="Id">Represents laser id.</param>
    /// <param name="Orientation">Represents laser orientation.</param>
    public record class Laser(int Id, LaserDirection Orientation) : TileEntity;

    /// <summary>
    /// Represents mine tile entitiy.
    /// </summary>
    /// <param name="Id">Represents mine id.</param>
    /// <param name="ExplosionRemainingTicks">Represents remaining ticks to explosion.</param>
    /// <remarks>
    /// <para>
    /// The value is <see langword="null"/>
    /// if the mine hasn't exploded yet.
    /// </para>
    /// </remarks>
    public record class Mine(int Id, int? ExplosionRemainingTicks) : TileEntity;
}
