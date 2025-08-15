using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddSingleton<InMemoryExternalCatalogGateway>();
        builder.Services.AddSingleton<IProductGateway>(sp => sp.GetRequiredService<InMemoryExternalCatalogGateway>());
        builder.Services.AddSingleton<IBranchGateway>(sp => sp.GetRequiredService<InMemoryExternalCatalogGateway>());
        builder.Services.AddSingleton<ICustomerGateway>(sp => sp.GetRequiredService<InMemoryExternalCatalogGateway>());
    }
}