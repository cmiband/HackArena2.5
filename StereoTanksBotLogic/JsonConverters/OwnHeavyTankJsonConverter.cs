using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents own heavy tank json converter.
/// </summary>
internal class OwnHeavyTankJsonConverter : JsonConverter<Tile.OwnHeavyTank>
{
    /// <inheritdoc/>
    public override Tile.OwnHeavyTank? ReadJson(JsonReader reader, Type objectType, Tile.OwnHeavyTank? existingValue, bool hasExistingValue, JsonSerializer serializer)
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

        return new Tile.OwnHeavyTank(
            jsonObject["ownerId"]!.ToObject<string>()!,
            jsonObject["direction"]!.ToObject<Direction>(),
            jsonObject["turret"]!.ToObject<OwnHeavyTankTurret>()!,
            jsonObject["health"]!.ToObject<int?>(),
            jsonObject["ticksToMine"]!.ToObject<int?>(),
            visibility);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Tile.OwnHeavyTank? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
