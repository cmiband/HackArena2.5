using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents game team json converter.
/// </summary>
internal class GameTeamJsonConverter : JsonConverter<GameTeam>
{
    /// <inheritdoc/>
    public override GameTeam? ReadJson(JsonReader reader, Type objectType, GameTeam? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        List<GamePlayer> players = new();
        foreach (var player in (JArray)jsonObject["players"]!)
        {
            players.Add(player.ToObject<GamePlayer>()!);
        }

        if (jsonObject.ContainsKey("score"))
        {
            return new OwnTeam(
                jsonObject["name"]!.ToObject<string>()!,
                jsonObject["color"]!.ToObject<uint>()!,
                jsonObject["score"]!.ToObject<int>()!,
                [.. players]);
        }
        else
        {
            return new EnemyTeam(
                jsonObject["name"]!.ToObject<string>()!,
                jsonObject["color"]!.ToObject<uint>()!,
                [.. players]);
        }
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, GameTeam? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
