using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.Models;

public record class OwnTeam(
    string Name,
    uint Color,
    int Score,
    GamePlayer[] Players)
    : GameTeam(
        Name,
        Color,
        Players);
