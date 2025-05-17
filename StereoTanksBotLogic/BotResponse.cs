using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Networking;
using StereoTanksBotLogic.Networking.Payloads;

namespace StereoTanksBotLogic;

/// <summary>
/// Represents bot response.
/// </summary>
public class BotResponse() : Packet
{
    /// <summary>
    /// Represents pass response.
    /// </summary>
    /// <remarks>
    /// Use this method to return pass action for your bot.
    /// Pass means that bot will do nothing for one game tick.
    /// </remarks>
    /// <returns>
    /// BotResponse representing pass action.
    /// </returns>
    public static BotResponse Pass()
    {
        PassPayload passPayload = new();
        return new BotResponse()
        {
            Type = passPayload.Type,
            Payload = JObject.FromObject(passPayload),
        };
    }

    /// <summary>
    /// Represents move response.
    /// </summary>
    /// <param name="tankMovement">
    /// Represents tank movement direction.
    /// </param>
    /// <remarks>
    /// Use this method to return move action for your bot.
    /// Move will change position of your tank one tile in specified direction.
    /// </remarks>
    /// <returns>
    /// BotResponse representing move action.
    /// </returns>
    public static BotResponse Move(MovementDirection tankMovement)
    {
        MovementPayload movementPayload = new(tankMovement);
        return new BotResponse()
        {
            Type = movementPayload.Type,
            Payload = JObject.FromObject(movementPayload),
        };
    }

    /// <summary>
    /// Represents rotate response.
    /// </summary>
    /// <param name="tankRotation">
    /// Represents tank rotation direction.
    /// </param>
    /// <param name="turretRotation">
    /// Represents turret rotation direction.
    /// </param>
    /// <remarks>
    /// Use this method to return rotate action for your bot.
    /// Rotate will move both tank and turret in the same tick.
    /// Passing null as rotation will result in no rotation of tank or turret.
    /// </remarks>
    /// <returns>
    /// BotResponse representing Rotate action.
    /// </returns>
    public static BotResponse Rotate(Rotation? tankRotation, Rotation? turretRotation)
    {
        RotationPayload rotationPayload = new()
        {
            TankRotation = tankRotation,
            TurretRotation = turretRotation,
        };
        return new BotResponse()
        {
            Type = rotationPayload.Type,
            Payload = JObject.FromObject(rotationPayload),
        };
    }

    /// <summary>
    /// Represents use ability response.
    /// </summary>
    /// <param name="abilityType">
    /// Represents ability type.
    /// </param>
    /// <remarks>
    /// Use this method to return use ability action for your bot.
    /// Use ability will use one of available abilities.
    /// </remarks>
    /// <returns>
    /// BotResponse representing UseAbility action.
    /// </returns>
    public static BotResponse UseAbility(AbilityType abilityType)
    {
        UseAbilityPayload abilityUsePayload = new(abilityType);
        return new BotResponse()
        {
            Type = abilityUsePayload.Type,
            Payload = JObject.FromObject(abilityUsePayload),
        };
    }

    /// <summary>
    /// Represents GoTo response.
    /// </summary>
    /// <param name="x">
    /// Represents target x coordinate.
    /// </param>
    /// <param name="y">
    /// Represents target y coordinate.
    /// </param>
    /// <param name="turretRotation">
    /// Represents turret rotation to be performed along move action.
    /// </param>
    /// <param name="costs">
    /// Represents pathfinding costs.
    /// </param>
    /// <param name="penalties">
    /// Represents pathfinding penalties.
    /// </param>
    /// <remarks>
    /// Use this method as a response every tick in order
    /// to move from your current position to target position.
    /// </remarks>
    /// <returns>
    /// BotResponse representing GoTo action.
    /// </returns>
    public static BotResponse GoTo(int x, int y, Rotation? turretRotation, Costs costs, Penalties penalties)
    {
        GoToPayload goToPayload = new(x, y, turretRotation, costs, penalties);
        return new BotResponse()
        {
            Type = goToPayload.Type,
            Payload = JObject.FromObject(goToPayload),
        };
    }

    /// <summary>
    /// Represents capture zone response.
    /// </summary>
    /// <remarks>
    /// Command the tank to attempt to capture the zone it is currently on.
    /// This is only effective if the tank is on a capturable zone tile.
    /// Use this method to increase your share in zone capture.
    /// For each tick this response is used share of your team will increase by one.
    /// If you use this method you cannot do anything else in this tick.
    /// </remarks>
    /// <returns>
    /// BotResponse representing capture zone action.
    /// </returns>
    public static BotResponse CaptureZone()
    {
        CaptureZonePayload captureZonePayload = new();
        return new BotResponse()
        {
            Type = captureZonePayload.Type,
            Payload = JObject.FromObject(captureZonePayload),
        };
    }
}
