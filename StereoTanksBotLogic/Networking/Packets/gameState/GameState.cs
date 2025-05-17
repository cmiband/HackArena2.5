using Newtonsoft.Json;
using StereoTanksBotLogic.JsonConverters;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents game state.
/// </summary>
/// <param name="Id">Represents game state id.</param>
/// <param name="Tick">Represents game tick.</param>
/// <param name="Players">Represents game players.</param>
/// <param name="Map">Represents game map.</param>
/// <param name="Zones">Represents game zones.</param>
[JsonConverter(typeof(GameStateJsonConverter))]
public record class GameState(
    string Id,
    string PlayerId,
    int Tick,
    GameTeam[] Teams,
    Tile[,] Map,
    Zone[] Zones);
