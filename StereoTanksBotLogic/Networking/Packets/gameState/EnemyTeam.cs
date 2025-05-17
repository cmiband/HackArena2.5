using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.JsonConverters;

public record class EnemyTeam(
    string Name,
    uint Color,
    GamePlayer[] Players) : GameTeam(Name, Color, Players);
