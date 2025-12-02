using System;

namespace ZPMLabs.Mailcow;

public sealed class MailcowOptions
{
    /// <summary>
    /// Base URL of the Mailcow instance, e.g. "https://mail.example.com".
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Mailcow API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Optional custom User-Agent header.
    /// </summary>
    public string UserAgent { get; init; } = "ZPMLabs-MailcowClient/1.0";

    /// <summary>
    /// Request timeout in seconds (used by HttpClient configuration).
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("Mailcow BaseUrl must be configured.");
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("Mailcow BaseUrl is not a valid absolute URI.");
        }

        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException("Mailcow ApiKey must be configured.");
        }
    }
}
