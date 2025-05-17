using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Models;
using static StereoTanksBotLogic.Models.ZoneStatus;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents zone json converter.
/// </summary>
public class ZoneJsonConverter : JsonConverter<Zone>
{
    /// <inheritdoc/>
    public override Zone? ReadJson(JsonReader reader, Type objectType, Zone? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        var x = jsonObject["x"]!.ToObject<int>()!;
        var y = jsonObject["y"]!.ToObject<int>()!;
        var width = jsonObject["width"]!.ToObject<int>()!;
        var height = jsonObject["height"]!.ToObject<int>()!;
        var index = jsonObject["index"]!.ToObject<int>()!;
        var shares = jsonObject["shares"]!.ToObject<Dictionary<string, float>>()!;

        return new Zone(index, x, y, width, height, shares);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Zone? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
