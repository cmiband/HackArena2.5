namespace StereoTanksBotLogic.Networking.Payloads
{
    using StereoTanksBotLogic.Enums;

    /// <summary>
    /// Represents movement payload.
    /// </summary>
    /// <param name="direction">Represents movement direction.</param>
    public class MovementPayload(MovementDirection direction)
    {
        /// <summary>
        /// Gets packet type.
        /// </summary>
        public PacketType Type => PacketType.Movement;

        /// <summary>
        /// Gets game state id.
        /// </summary>
        /// <remarks>
        /// GameStateId is required in all bot responces.
        /// This api wrapper automatically sets correct GameStateId.
        /// </remarks>
        public string? GameStateId { get; init; }

        /// <summary>
        /// Gets movement direction.
        /// </summary>
        public MovementDirection Direction { get; } = direction;
    }
}
