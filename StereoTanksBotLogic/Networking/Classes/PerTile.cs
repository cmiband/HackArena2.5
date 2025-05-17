namespace StereoTanksBotLogic;

/// <summary>
/// Represents pathfinding penalties for each tile.
/// </summary>
/// <param name="x">Represents tile x coordinate.</param>
/// <param name="y">Represents tile y coordinate.</param>
/// <param name="penalty">Represents tile penalty.</param>
public class PerTile(int x, int y, float penalty)
{
    /// <summary>
    /// Gets or sets tile x coordinate.
    /// </summary>
    public int X { get; set; } = x;

    /// <summary>
    /// Gets or sets tile y coordinate.
    /// </summary>
    public int Y { get; set; } = y;

    /// <summary>
    /// Gets or sets tile penalty.
    /// </summary>
    public float Penalty { get; set; } = penalty;
}
