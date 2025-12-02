using ZPMLabs.Mailcow.Dto;

namespace ZPMLabs.Mailcow.Abstractions;

public interface IDomainsApiClient
{
    Task<IReadOnlyList<DomainDto>> ListDomainsAsync(CancellationToken cancellationToken = default);

    Task<object?> CreateDomainAsync(DomainDto domain, CancellationToken cancellationToken = default);
}
