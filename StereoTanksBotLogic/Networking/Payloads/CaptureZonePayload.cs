namespace StereoTanksBotLogic.Networking.Payloads;

/// <summary>
/// Represents pass payload.
/// </summary>
public class CaptureZonePayload
{
    /// <summary>
    /// Gets packet type.
    /// </summary>
    public PacketType Type => PacketType.CaptureZone;

    /// <summary>
    /// Gets game state id.
    /// </summary>
    /// <remarks>
    /// GameStateId is required in all bot responces.
    /// This api wrapper automatically sets correct GameStateId.
    /// </remarks>
    public string? GameStateId { get; init; }
}
