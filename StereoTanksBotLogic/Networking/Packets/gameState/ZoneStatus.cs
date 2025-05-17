namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents zone status.
/// </summary>
public abstract record class ZoneStatus
{
    /// <summary>
    /// Represents neutral zone status.
    /// </summary>
    public record class Neutral : ZoneStatus;

    /// <summary>
    /// Represents being captured zone status.
    /// </summary>
    /// <param name="RemainingTicks">Represents remaining ticks to capture.</param>
    /// <param name="PlayerId">Represents capturer player id.</param>
    public record class BeingCaptured(int RemainingTicks, string PlayerId) : ZoneStatus;

    /// <summary>
    /// Represents captured zone status.
    /// </summary>
    /// <param name="PlayerId">Represents capturer player id.</param>
    public record class Captured(string PlayerId) : ZoneStatus;

    /// <summary>
    /// Represents contested zone status.
    /// </summary>
    /// <param name="CapturedById">Represents capturer id.</param>
    public record class BeingContested(string? CapturedById) : ZoneStatus;

    /// <summary>
    /// Represents being retaken zone status.
    /// </summary>
    /// <param name="RemainingTicks">Represents ticks remaining to retake.</param>
    /// <param name="CapturedById">Represents capturer id.</param>
    /// <param name="RetakenById">Represents retaker id.</param>
    public record class BeingRetaken(int RemainingTicks, string CapturedById, string RetakenById) : ZoneStatus;
}
