using Newtonsoft.Json;
using StereoTanksBotLogic.JsonConverters;

namespace StereoTanksBotLogic.Models;

/// <summary>
/// Represents zone.
/// </summary>
/// <param name="Index">Represents zone index.</param>
/// <param name="X">Represents zone x position.</param>
/// <param name="Y">Represents zone y position.</param>
/// <param name="Width">Represents zone width.</param>
/// <param name="Height">Represents zone height.</param>
/// <param name="Status">Represents zone capture status.</param>
[JsonConverter(typeof(ZoneJsonConverter))]
public record class Zone(int Index, int X, int Y, int Width, int Height, Dictionary<string, float> Shares);
