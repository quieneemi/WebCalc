using System.Text.Json.Serialization;

namespace SmartCalc.Core.Models;

public struct CalcOperation
{
    [JsonPropertyName("expression")]
    public string Expression { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; }
}