using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents game player json converter.
/// </summary>
internal class GamePlayerJsonConverter : JsonConverter<GamePlayer>
{
    /// <inheritdoc/>
    public override GamePlayer? ReadJson(JsonReader reader, Type objectType, GamePlayer? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        if (jsonObject.ContainsKey("ticksToRegen"))
        {
            return new OwnPlayer(
                jsonObject["id"]!.ToObject<string>()!,
                jsonObject["ping"]!.ToObject<int>()!,
                jsonObject["ticksToRegen"]!.ToObject<int?>()!);
        }
        else
        {
            return new EnemyPlayer(
                jsonObject["id"]!.ToObject<string>()!,
                jsonObject["ping"]!.ToObject<int>()!);
        }
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, GamePlayer? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
