using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents own light tank json converter.
/// </summary>
internal class OwnLightTankJsonConverter : JsonConverter<Tile.OwnLightTank>
{
    /// <inheritdoc/>
    public override Tile.OwnLightTank? ReadJson(JsonReader reader, Type objectType, Tile.OwnLightTank? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        var rawVisibility = jsonObject["visibility"]!.ToObject<string[]>()!;
        var width = rawVisibility.Length;
        var height = rawVisibility[0].Length;
        bool[,] visibility = new bool[width, height];
        for (int i = 0; i < rawVisibility.Length; i++)
        {
            for (int j = 0; j < rawVisibility[i].Length; j++)
            {
                visibility[i, j] = rawVisibility[i][j] == '1';
            }
        }

        return new Tile.OwnLightTank(
            jsonObject["ownerId"]!.ToObject<string>()!,
            jsonObject["direction"]!.ToObject<Direction>(),
            jsonObject["turret"]!.ToObject<OwnLightTankTurret>()!,
            jsonObject["health"]!.ToObject<int>(),
            jsonObject["ticksToRadar"]!.ToObject<int?>(),
            jsonObject["isUsingRadar"]!.ToObject<bool>(),
            visibility);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Tile.OwnLightTank? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
