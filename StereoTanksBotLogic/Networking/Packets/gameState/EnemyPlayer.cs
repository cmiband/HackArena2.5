namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents enemy player.
/// </summary>
/// <param name="Id">
/// Represents id of an enemy player.
/// </param>
/// <param name="Nickname">
/// Represents nickname of an enemy player.
/// </param>
/// <param name="Color">
/// Represents color of an enemy player.
/// </param>
/// <param name="Ping">
/// Represents ping of an enemy player.
/// </param>
public record class EnemyPlayer(
string Id,
int Ping)
: GamePlayer(
    Id,
    Ping);
