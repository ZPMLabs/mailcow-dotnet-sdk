using ZPMLabs.Mailcow.Abstractions;

namespace ZPMLabs.Mailcow;

public sealed class MailcowClient : IMailcowClient
{
    public IDomainsApiClient Domains { get; }
    public IMailboxesApiClient Mailboxes { get; }

    public MailcowClient(
        IDomainsApiClient domainsApiClient,
        IMailboxesApiClient mailboxesApiClient)
    {
        Domains = domainsApiClient;
        Mailboxes = mailboxesApiClient;
    }
}
