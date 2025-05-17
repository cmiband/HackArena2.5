using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Networking.Payloads;

/// <summary>
/// Represents an ability use payload.
/// </summary>
/// <param name="abilityType">The ability type.</param>
public class UseAbilityPayload(AbilityType abilityType)
{
    /// <summary>
    /// Gets packet type.
    /// </summary>
    public PacketType Type => PacketType.AbilityUse;

    /// <summary>
    /// Gets game state id.
    /// </summary>
    /// <remarks>
    /// GameStateId is required in all bot responces.
    /// This api wrapper automatically sets correct GameStateId.
    /// </remarks>
    public string? GameStateId { get; init; }

    /// <summary>
    /// Gets ability type.
    /// </summary>
    public AbilityType AbilityType { get; } = abilityType;
}
