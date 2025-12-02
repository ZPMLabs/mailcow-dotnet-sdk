namespace ZPMLabs.Mailcow.Abstractions;

public interface IMailcowClient
{
    IDomainsApiClient Domains { get; }
    IMailboxesApiClient Mailboxes { get; }
}
