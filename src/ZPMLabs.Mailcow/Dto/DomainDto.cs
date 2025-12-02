namespace ZPMLabs.Mailcow.Dto;

public sealed class DomainDto
{
    public required string Domain { get; init; }

    public bool Active { get; init; } = true;

    public int? MaxMailboxes { get; init; }

    public int? MaxAliases { get; init; }

    /// <summary>
    /// Quota in megabytes.
    /// </summary>
    public int? QuotaMb { get; init; }

    public IDictionary<string, object?> ToPayload()
    {
        var payload = new Dictionary<string, object?>
        {
            ["domain"] = Domain,
            ["active"] = Active ? "1" : "0",
            ["max_mailboxes"] = MaxMailboxes,
            ["max_aliases"] = MaxAliases,
            ["quota"] = QuotaMb
        };

        // Remove nulls
        return payload.Where(kv => kv.Value is not null)
                      .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
