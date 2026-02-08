using System.Net;
using System.Text.Json;
using MyRent.Exceptions.MyRentApi;
using MyRent.Interfaces.Services;
using MyRent.Models.Dtos;
using MyRent.Utilities.Json;

namespace MyRent.Services;
public sealed class MyRentClientService(HttpClient httpClient) : IMyRentClientService
{
    /// <summary>
    /// Asynchronously retrieves a list of property items.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of property items, or null if no properties are available.</returns>
    public async Task<IReadOnlyList<PropertyItem>?> GetPropertiesAsync(CancellationToken cancellationToken) =>
        await GetListAsync<PropertyItem>("objects/simple_list", cancellationToken);

    /// <summary>
    /// Retrieves the details of a specific property based on the provided property identifier.
    /// </summary>
    /// <param name="idHash">The unique identifier (hash) of the property to retrieve details for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous task that returns the property details as a <see cref="PropertyItem"/> object.</returns>
    public async Task<PropertyItem> GetPropertyDetailsAsync(string idHash, CancellationToken cancellationToken) =>
        await GetOneAsync<PropertyItem>($"objects/simple_details?id_hash={Uri.EscapeDataString(idHash)}", cancellationToken);

    /// Retrieves a list of property pictures based on the provided property identifier.
    /// <param name="idHash">The unique identifier hash of the property for which pictures need to be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
    /// <returns>A read-only list of property pictures, or null if no pictures are found.</returns>
    public async Task<IReadOnlyList<PropertyPicture>?> GetPicturesAsync(string idHash, CancellationToken cancellationToken) =>
        await GetListAsync<PropertyPicture>($"objects/get_pictures_links?id_hash={Uri.EscapeDataString(idHash)}", cancellationToken);

    /// Retrieves a single item of the specified type from the given relative URL using an asynchronous HTTP GET request.
    /// <typeparam name="T">The type of the item to retrieve.</typeparam>
    /// <param name="relativeUrl">The relative URL to send the HTTP GET request to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation, if needed.</param>
    /// <returns>The deserialized object of type T retrieved from the response.
    /// Throws a MyRentApiException if the HTTP request fails or the deserialization is unsuccessful.</returns>
    private async Task<T> GetOneAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(relativeUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw await CreateException(response, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return DeserializeOne<T>(json);
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified relative URL and retrieves a list of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list that are retrieved.</typeparam>
    /// <param name="relativeUrl">The relative URL endpoint to send the GET request to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of items of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MyRentApiException">
    /// Thrown when the request does not succeed or if the response has an unexpected payload format.
    /// </exception>
    private async Task<IReadOnlyList<T>> GetListAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(relativeUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw await CreateException(response, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return DeserializeList<T>(json);
    }

    /// <summary>
    /// Deserializes a JSON string into a single instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type into which the JSON string will be deserialized.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An instance of type <typeparamref name="T"/> deserialized from the JSON string.</returns>
    /// <exception cref="MyRent.Exceptions.MyRentApi.MyRentApiException">
    /// Thrown when the JSON structure is invalid, contains unsupported payloads, or when the deserialized object is null.
    /// </exception>
    private static T DeserializeOne<T>(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        
        JsonElement element = root;

        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var dataEl))
            element = dataEl;

        if (element.ValueKind == JsonValueKind.Array)
        {
            if (element.GetArrayLength() == 0)
                throw new MyRentApiException(HttpStatusCode.InternalServerError, $"Empty array payload: {json}");

            element = element[0];
        }

        if (element.ValueKind != JsonValueKind.Object)
            throw new MyRentApiException(HttpStatusCode.InternalServerError, $"Unsupported payload shape: {json}");

        var value = JsonSerializer.Deserialize<T>(element.GetRawText(), JsonDefaults.Options);
        if (value is null)
            throw new MyRentApiException(HttpStatusCode.InternalServerError, $"Deserialized null payload: {json}");

        return value;
    }

    /// <summary>
    /// Deserializes a JSON string into a read-only list of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the objects contained in the deserialized list.</typeparam>
    /// <param name="json">The JSON string representing the list of objects to deserialize.</param>
    /// <returns>
    /// A read-only list of objects of type <typeparamref name="T"/>. If the JSON represents an empty list,
    /// an empty read-only list is returned. If the JSON represents a single object instead of a list,
    /// a list containing that single object is returned.
    /// </returns>
    /// <exception cref="MyRent.Exceptions.MyRentApi.MyRentApiException">
    /// Thrown when the JSON is not in a supported format for deserialization into a list.
    /// </exception>
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

    /// Creates an instance of a `MyRentApiException` based on the given HTTP response.
    /// <param name="response">The HTTP response message that contains the status code and content to create the exception.</param>
    /// <param name="cancellationToken">A cancellation token to monitor for cancellation requests.</param>
    /// <return>
    /// A task that represents the asynchronous operation. The task result contains a `MyRentApiException`
    /// with the status code and the response body as the error message.
    /// </return>
    private static async Task<MyRentApiException> CreateException(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new MyRentApiException(response.StatusCode, body);
    }
}
