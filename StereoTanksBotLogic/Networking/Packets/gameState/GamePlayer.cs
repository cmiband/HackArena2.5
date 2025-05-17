using Newtonsoft.Json;
using StereoTanksBotLogic.JsonConverters;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents game player.
/// </summary>
/// <param name="Id">Represents id of a game player.</param>
/// <param name="Nickname">Represents nickname of a game player.</param>
/// <param name="Color">Represents color of a game player.</param>
/// <param name="Ping">Represents ping of a game player.</param>
[JsonConverter(typeof(GamePlayerJsonConverter))]
public abstract record class GamePlayer(
    string Id,
    int Ping);
