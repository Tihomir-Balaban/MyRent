using System.Text.Json.Serialization;

namespace MyRent.Models.Dtos;

public sealed  class PropertyPicture
{
    [JsonPropertyName("picture_link")]
    public string PictureLink { get; init; } = string.Empty;
}