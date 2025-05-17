using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Networking.Payloads
{
    /// <summary>
    /// Represents go to payload.
    /// </summary>
    /// <param name="turretRotation">Represents turret rotation direction.</param>
    public class GoToPayload(int x, int y, Rotation? turretRotation, Costs costs, Penalties penalties)
    {
        /// <summary>
        /// Gets packet type.
        /// </summary>
        public PacketType Type => PacketType.GoTo;

        /// <summary>
        /// Gets game state id.
        /// </summary>
        /// <remarks>
        /// GameStateId is required in all bot responces.
        /// This api wrapper automatically sets correct GameStateId.
        /// </remarks>
        public string? GameStateId { get; init; }

        /// <summary>
        /// Gets the destination x coordiante.
        /// </summary>
        public int X { get; } = x;

        /// <summary>
        /// Gets the destination y coordiante.
        /// </summary>
        public int Y { get; } = y;

        /// <summary>
        /// Gets the turret rotation.
        /// </summary>
        public Rotation? TurretRotation { get; } = turretRotation;

        /// <summary>
        /// Gets the pathfinding costs.
        /// </summary>
        public Costs Costs { get; } = costs;

        /// <summary>
        /// Gets the pathfinding penalties.
        /// </summary>
        public Penalties Penalties { get; } = penalties;
    }
}
