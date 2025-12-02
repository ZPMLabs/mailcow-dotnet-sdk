using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZPMLabs.Mailcow.Abstractions;
using ZPMLabs.Mailcow.Dto;
using ZPMLabs.Mailcow.Exceptions;

namespace ZPMLabs.Mailcow.Api;

public sealed class MailboxesApiClient : IMailboxesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly MailcowOptions _options;
    private readonly ILogger<MailboxesApiClient>? _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public MailboxesApiClient(
        HttpClient httpClient,
        IOptions<MailcowOptions> options,
        ILogger<MailboxesApiClient>? logger = null)
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

    public async Task<IReadOnlyList<MailboxDto>> ListMailboxesAsync(
        string? domain = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = "/api/v1/get/mailbox/all";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending Mailcow ListMailboxes request.");
            throw new MailcowRequestException("Error sending Mailcow ListMailboxes request.", request, ex);
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("Mailcow ListMailboxes returned non-success status {StatusCode}: {Content}",
                response.StatusCode, content);

            throw new MailcowResponseException(
                $"Mailcow ListMailboxes failed with status {(int)response.StatusCode}.",
                request,
                response);
        }

        try
        {
            var list = JsonSerializer.Deserialize<List<MailboxDto>>(content, JsonOptions) ?? new List<MailboxDto>();

            if (domain is null)
                return list;

            return list.Where(m => m.Domain == domain).ToList();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to deserialize Mailcow ListMailboxes response: {Content}", content);
            throw new MailcowResponseException(
                "Failed to deserialize Mailcow ListMailboxes response.",
                request,
                response,
                ex);
        }
    }

    public async Task<object?> CreateMailboxAsync(MailboxDto mailbox, CancellationToken cancellationToken = default)
    {
        var requestUri = "/api/v1/add/mailbox";
        var payload = mailbox.ToPayload();

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
            _logger?.LogError(ex, "Error sending Mailcow CreateMailbox request.");
            throw new MailcowRequestException("Error sending Mailcow CreateMailbox request.", request, ex);
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("Mailcow CreateMailbox returned non-success status {StatusCode}: {Content}",
                response.StatusCode, content);

            throw new MailcowResponseException(
                $"Mailcow CreateMailbox failed with status {(int)response.StatusCode}.",
                request,
                response);
        }

        try
        {
            return JsonSerializer.Deserialize<object?>(content, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to deserialize Mailcow CreateMailbox response: {Content}", content);
            throw new MailcowResponseException(
                "Failed to deserialize Mailcow CreateMailbox response.",
                request,
                response,
                ex);
        }
    }
}
