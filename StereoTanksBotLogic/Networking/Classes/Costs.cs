namespace StereoTanksBotLogic;

/// <summary>
/// Represents pathfinding costs.
/// </summary>
public class Costs(float forward, float backward, float rotate)
{
    /// <summary>
    /// Gets or sets cost incurred for moving forward one tile. Higher values make moving forward less preferable.
    /// </summary>
    public float Forward { get; set; } = forward;

    /// <summary>
    /// Gets or sets cost incurred for moving backward one tile. Higher values make moving backward less preferable.
    /// </summary>
    public float Backward { get; set; } = backward;

    /// <summary>
    /// Gets or sets cost incurred for rotating the tank (either left or right). Higher values make rotation less preferable.
    /// </summary>
    public float Rotate { get; set; } = rotate;
}
