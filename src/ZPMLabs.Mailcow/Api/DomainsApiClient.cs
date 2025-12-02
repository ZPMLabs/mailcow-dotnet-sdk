using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZPMLabs.Mailcow.Abstractions;
using ZPMLabs.Mailcow.Dto;
using ZPMLabs.Mailcow.Exceptions;

namespace ZPMLabs.Mailcow.Api;

public sealed class DomainsApiClient : IDomainsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly MailcowOptions _options;
    private readonly ILogger<DomainsApiClient>? _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public DomainsApiClient(
        HttpClient httpClient,
        IOptions<MailcowOptions> options,
        ILogger<DomainsApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _options.Validate();
        _logger = logger;

        _httpClient.BaseAddress ??= new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<IReadOnlyList<DomainDto>> ListDomainsAsync(CancellationToken cancellationToken = default)
    {
        var requestUri = "/api/v1/get/domain/all";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending Mailcow ListDomains request.");
            throw new MailcowRequestException("Error sending Mailcow ListDomains request.", request, ex);
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("Mailcow ListDomains returned non-success status {StatusCode}: {Content}",
                response.StatusCode, content);

            throw new MailcowResponseException(
                $"Mailcow ListDomains failed with status {(int)response.StatusCode}.",
                request,
                response);
        }

        try
        {
            // Mailcow returns an array of domain info objects.
            var data = JsonSerializer.Deserialize<List<DomainDto>>(content, JsonOptions);
            return data ?? new List<DomainDto>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to deserialize Mailcow ListDomains response: {Content}", content);
            throw new MailcowResponseException(
                "Failed to deserialize Mailcow ListDomains response.",
                request,
                response,
                ex);
        }
    }

    public async Task<object?> CreateDomainAsync(DomainDto domain, CancellationToken cancellationToken = default)
    {
        var requestUri = "/api/v1/add/domain";

        var payload = domain.ToPayload();

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending Mailcow CreateDomain request.");
            throw new MailcowRequestException("Error sending Mailcow CreateDomain request.", request, ex);
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("Mailcow CreateDomain returned non-success status {StatusCode}: {Content}",
                response.StatusCode, content);

            throw new MailcowResponseException(
                $"Mailcow CreateDomain failed with status {(int)response.StatusCode}.",
                request,
                response);
        }

        // Mailcow obično vraća JSON sa statusom / message
        try
        {
            return JsonSerializer.Deserialize<object?>(content, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to deserialize Mailcow CreateDomain response: {Content}", content);
            throw new MailcowResponseException(
                "Failed to deserialize Mailcow CreateDomain response.",
                request,
                response,
                ex);
        }
    }
}
