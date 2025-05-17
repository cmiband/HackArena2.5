using System.Collections.ObjectModel;

namespace StereoTanksBotLogic;

/// <summary>
/// Represents pathfinding penalties.
/// </summary>
public class Penalties(float? blindly, float? bullet, float? mine, float? laser, float? tank, PerTile[]? perTile)
{
    /// <summary>
    /// Gets or sets penalty for the first move if it's considered 'blind' (e.g., moving backward while the turret faces forward, or moving without visibility).
    /// </summary>
    /// <remarks>
    /// If this penalty is null pathfinding does not apply penalty for blind movement.
    /// </remarks>
    public float? Blindly { get; set; } = blindly;

    /// <summary>
    /// Gets or sets penalty for pathfinding through a tile currently occupied by a bullet.
    /// </summary>
    /// <remarks>
    /// If this penalty is null pathfinding does not apply penalty for movement trough bullet.
    /// </remarks>
    public float? Bullet { get; set; } = bullet;

    /// <summary>
    /// Gets or sets penalty for pathfinding through a tile currently occupied by a mine.
    /// </summary>
    /// <remarks>
    /// If this penalty is null pathfinding does not apply penalty for movement trough mine.
    /// </remarks>
    public float? Mine { get; set; } = mine;

    /// <summary>
    /// Gets or sets penalty for pathfinding through a tile currently affected by a laser beam.
    /// </summary>
    /// <remarks>
    /// If this penalty is null pathfinding does not apply penalty for movement trough laser.
    /// </remarks>
    public float? Laser { get; set; } = laser;

    /// <summary>
    /// Gets or sets penalty for pathfinding through a tile currently occupied by another tank.
    /// </summary>
    /// <remarks>
    /// If this penalty is null pathfinding does not apply penalty for movement trough another tank.
    /// </remarks>
    public float? Tank { get; set; } = tank;

    /// <summary>
    /// Gets or sets list of specific tiles with custom, additional penalties.
    /// </summary>
    /// <remarks>
    /// Useful for scenarios like avoiding pre-discovered mine locations or other tactical no-go zones.
    /// If null or an empty list, no per-tile penalties are applied from this list.
    /// </remarks>
    public PerTile[]? PerTile { get; set; } = perTile;
}
