using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Networking.Payloads;

/// <summary>
/// Represents rotation payload.
/// </summary>
public class RotationPayload
{
    /// <summary>
    /// Gets packet type.
    /// </summary>
    public PacketType Type => PacketType.Rotation;

    /// <summary>
    /// Gets game state id.
    /// </summary>
    /// <remarks>
    /// GameStateId is required in all bot responces.
    /// This api wrapper automatically sets correct GameStateId.
    /// </remarks>
    public string? GameStateId { get; init; }

    /// <summary>
    /// Gets the tank rotation.
    /// </summary>
    /// <remarks>
    /// If the value is <see langword="null"/>,
    /// the tank rotation is not changed.
    /// </remarks>
    public Rotation? TankRotation { get; init; }

    /// <summary>
    /// Gets the turret rotation.
    /// </summary>
    /// <remarks>
    /// If the value is <see langword="null"/>,
    /// the turret rotation is not changed.
    /// </remarks>
    public Rotation? TurretRotation { get; init; }
}
