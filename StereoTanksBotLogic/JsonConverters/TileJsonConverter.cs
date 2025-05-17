    using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;
using static StereoTanksBotLogic.Models.Tile;

namespace StereoTanksBotLogic.JsonConverters;

/// <summary>
/// Represents tile json converter.
/// </summary>
internal class TileJsonConverter : JsonConverter<Tile>
{
    /// <inheritdoc/>
    public override Tile? ReadJson(JsonReader reader, Type objectType, Tile? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonArray = JArray.Load(reader);

        List<TileEntity> entities = new();
        foreach (var tileEntity in jsonArray)
        {
            switch (tileEntity["type"]!.ToObject<string>()!)
            {
                case "wall":
                    {
                        var rawPayload = (JObject)tileEntity["payload"]!;
                        entities.Add(rawPayload.ToObject<Wall>()!);
                        break;
                    }

                case "tank":
                    {
                        var rawPayload = (JObject)tileEntity["payload"]!;
                        if (rawPayload.ContainsKey("health"))
                        {
                            if (rawPayload.ContainsKey("ticksToRadar") && rawPayload.ContainsKey("isUsingRadar"))
                            {
                                entities.Add(rawPayload.ToObject<OwnLightTank>()!);
                            }
                            else
                            {
                                entities.Add(rawPayload.ToObject<OwnHeavyTank>()!);
                            }
                        }
                        else
                        {
                            if (rawPayload["type"]!.ToObject<TankType>() == TankType.Light)
                            {
                                entities.Add(rawPayload.ToObject<EnemyLightTank>()!);
                            }
                            else
                            {
                                entities.Add(rawPayload.ToObject<EnemyHeavyTank>()!);
                            }
                        }

                        break;
                    }

                case "bullet":
                    {
                        var rawPayload = (JObject)tileEntity["payload"]!;
                        entities.Add(rawPayload.ToObject<Bullet>()!);
                        break;
                    }

                case "laser":
                    {
                        var rawPayload = (JObject)tileEntity["payload"]!;
                        entities.Add(rawPayload.ToObject<Laser>()!);
                        break;
                    }

                case "mine":
                    {
                        var rawPayload = (JObject)tileEntity["payload"]!;
                        entities.Add(rawPayload.ToObject<Mine>()!);
                        break;
                    }
            }
        }

        return new Tile(null, [.. entities]);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Tile? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }
}
