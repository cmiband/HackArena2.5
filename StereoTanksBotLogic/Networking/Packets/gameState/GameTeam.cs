using Newtonsoft.Json;
using StereoTanksBotLogic.JsonConverters;
using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.Models;

[JsonConverter(typeof(GameTeamJsonConverter))]
public abstract record class GameTeam(
    string Name,
    uint Color,
    GamePlayer[] Players);
