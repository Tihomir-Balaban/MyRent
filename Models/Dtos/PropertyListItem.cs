using System.Text.Json.Serialization;

namespace MyRent.Models.Dtos;

public sealed class PropertyListItem
{
    [JsonPropertyName("id_hash")]
    public string IdHash { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("picutre_main_url")]
    public string? PictureMainUrl { get; init; }

    [JsonPropertyName("instant_booking")]
    public string? InstantBooking { get; init; }

    [JsonPropertyName("pay_card")]
    public string? PayCard { get; init; }

    [JsonPropertyName("pay_casche")]
    public string? PayCash { get; init; }

    [JsonPropertyName("pay_iban")]
    public string? PayIban { get; init; }

    [JsonPropertyName("object_type")]
    public string? ObjectType { get; init; }

    [JsonPropertyName("latitude")]
    public string? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; init; }

    [JsonPropertyName("adress")]
    public string? Address { get; init; }

    [JsonPropertyName("city_zip")]
    public string? CityZip { get; init; }

    [JsonPropertyName("city_name")]
    public string? CityName { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("can_sleep_optimal")]
    public int CanSleepOptimal { get; init; }

    [JsonPropertyName("can_sleep_max")]
    public int CanSleepMax { get; init; }

    [JsonPropertyName("beds")]
    public double? Beds { get; init; }

    [JsonPropertyName("bathrooms")]
    public double? Bathrooms { get; init; }

    [JsonPropertyName("bedrooms")]
    public double? Bedrooms { get; init; }

    [JsonPropertyName("livingrooms")]
    public double? LivingRooms { get; init; }

    [JsonPropertyName("toilets")]
    public double? Toilets { get; init; }

    [JsonPropertyName("can_sleep_optimal1")]
    public int? CanSleepOptimal1 { get; init; }

    [JsonPropertyName("pets_number")]
    public int? PetsNumber { get; init; }

    [JsonPropertyName("pet_price")]
    public double? PetPrice { get; init; }

    [JsonPropertyName("check_in")]
    public string? CheckIn { get; init; }

    [JsonPropertyName("check_out")]
    public string? CheckOut { get; init; }

    [JsonPropertyName("check_in_until")]
    public string? CheckInUntil { get; init; }

    [JsonPropertyName("classification_star")]
    public int? ClassificationStar { get; init; }
}