namespace ZPMLabs.Mailcow.Dto;

public sealed class MailboxDto
{
    public required string LocalPart { get; init; }     // "info"
    public required string Domain { get; init; }        // "example.com"
    public required string Password { get; init; }
    public int QuotaMb { get; init; } = 1024;
    public bool Active { get; init; } = true;
    public string? DisplayName { get; init; }

    public string Address => $"{LocalPart}@{Domain}";

    public IDictionary<string, object?> ToPayload()
    {
        var payload = new Dictionary<string, object?>
        {
            ["local_part"] = LocalPart,
            ["domain"] = Domain,
            ["password"] = Password,
            ["password2"] = Password,
            ["quota"] = QuotaMb,
            ["active"] = Active ? "1" : "0",
            ["name"] = DisplayName
        };

        return payload.Where(kv => kv.Value is not null)
                      .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
