using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using ZPMLabs.Mailcow.Abstractions;
using ZPMLabs.Mailcow.Api;

namespace ZPMLabs.Mailcow.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Mailcow client and API clients with HttpClientFactory and options binding.
    /// </summary>
    public static IServiceCollection AddMailcowClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSectionName = "Mailcow")
    {
        services.AddOptions<MailcowOptions>()
            .Bind(configuration.GetSection(configurationSectionName))
            .Validate(o =>
            {
                try
                {
                    o.Validate();
                    return true;
                }
                catch
                {
                    return false;
                }
            }, "Mailcow options are invalid.");

        // Typed HttpClients for APIs
        services.AddHttpClient<IDomainsApiClient, DomainsApiClient>();
        services.AddHttpClient<IMailboxesApiClient, MailboxesApiClient>();

        // Facade
        services.AddSingleton<IMailcowClient, MailcowClient>();

        return services;
    }
}
