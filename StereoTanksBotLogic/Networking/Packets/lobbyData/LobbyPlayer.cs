using StereoTanksBotLogic.Enums;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents lobby player.
/// </summary>
/// <param name="Id">Represents lobby player id.</param>
/// <param name="TankType">Represents lobby player tank type.</param>
public record class LobbyPlayer(string Id, TankType TankType);
