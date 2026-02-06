using System.Net;
using System.Text.Json;
using MyRent.Exceptions.MyRentApi;
using MyRent.Interfaces.MyRentApi;
using MyRent.Models.Dtos;
using MyRent.Utilities.Json;

namespace MyRent.MyRentApi;
public sealed class MyRentClient(HttpClient httpClient) : IMyRentClient
{
    // private readonly HttpClient _httpClient = httpClient;
    
    // public async Task<IReadOnlyList<PropertyListItem>?> GetPropertiesAsync(CancellationToken cancellationToken)
    // {
    //     var response = await _httpClient.GetAsync("objects/simple_list", cancellationToken);
    //
    //     if (!response.IsSuccessStatusCode)
    //         throw await CreateException(response);
    //
    //     var data = await response.Content
    //         .ReadFromJsonAsync<List<PropertyListItem>>(cancellationToken: cancellationToken);
    //
    //     return data;
    // }
    
    // public async Task<IReadOnlyList<PropertyDetails>> GetPropertyDetailsAsync(string idHash, CancellationToken cancellationToken)
    // {
    //     var response = await _httpClient.GetAsync(
    //         $"objects/simple_details?id_hash={idHash}", cancellationToken);
    //
    //     if (!response.IsSuccessStatusCode)
    //         throw await CreateException(response);
    //     
    //     var data = await response.Content
    //         .ReadFromJsonAsync<IReadOnlyList<PropertyDetails>>(cancellationToken: cancellationToken);
    //
    //     if (data is null)
    //         throw new MyRentApiException(
    //             HttpStatusCode.InternalServerError,
    //             "Empty response from simple_details");
    //
    //     return data;
    // }

    // public async Task<IReadOnlyList<PropertyPicture>?> GetPicturesAsync(string idHash, CancellationToken cancellationToken)
    // {
    //     var response = await _httpClient.GetAsync(
    //         $"objects/get_pictures_links?id_hash={idHash}", cancellationToken);
    //
    //     if (!response.IsSuccessStatusCode)
    //         throw await CreateException(response);
    //
    //     var data = await response.Content
    //         .ReadFromJsonAsync<List<PropertyPicture>>(cancellationToken: cancellationToken);
    //
    //     return data;
    // }
    public async Task<IReadOnlyList<PropertyListItem>?> GetPropertiesAsync(CancellationToken cancellationToken) =>
        await GetListAsync<PropertyListItem>("objects/simple_list", cancellationToken);
    
    public async Task<IReadOnlyList<PropertyDetails>> GetPropertyDetailsAsync(string idHash, CancellationToken cancellationToken) =>
        await GetListAsync<PropertyDetails>($"objects/simple_details?id_hash={Uri.EscapeDataString(idHash)}", cancellationToken);

    public async Task<IReadOnlyList<PropertyPicture>?> GetPicturesAsync(string idHash, CancellationToken cancellationToken) =>
        await GetListAsync<PropertyPicture>($"objects/get_pictures_links?id_hash={Uri.EscapeDataString(idHash)}", cancellationToken);
    
    private async Task<IReadOnlyList<T>> GetListAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(relativeUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw await CreateException(response, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return DeserializeList<T>(json);
    }

    private static IReadOnlyList<T> DeserializeList<T>(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var element = root;

        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var dataEl))
            element = dataEl;

        if (element.ValueKind == JsonValueKind.Object)
        {
            var single = JsonSerializer.Deserialize<T>(element.GetRawText(), JsonDefaults.Options);
            return single is null ? Array.Empty<T>() : new[] { single };
        }

        if (element.ValueKind != JsonValueKind.Array)
            throw new MyRentApiException(HttpStatusCode.InternalServerError, $"Unsupported list payload shape: {json}");

        var list = JsonSerializer.Deserialize<List<T>>(element.GetRawText(), JsonDefaults.Options);
        return list ?? (IReadOnlyList<T>) new List<T>().DefaultIfEmpty();
    }

    private static async Task<MyRentApiException> CreateException(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new MyRentApiException(response.StatusCode, body);
    }
}
