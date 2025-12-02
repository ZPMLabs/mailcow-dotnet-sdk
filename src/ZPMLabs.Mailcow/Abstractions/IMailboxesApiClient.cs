using ZPMLabs.Mailcow.Dto;

namespace ZPMLabs.Mailcow.Abstractions;

public interface IMailboxesApiClient
{
    Task<IReadOnlyList<MailboxDto>> ListMailboxesAsync(
        string? domain = null,
        CancellationToken cancellationToken = default);

    Task<object?> CreateMailboxAsync(MailboxDto mailbox, CancellationToken cancellationToken = default);
}
