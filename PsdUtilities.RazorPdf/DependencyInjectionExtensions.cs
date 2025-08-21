using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

using PsdUtilities.RazorPdf.Playwright;

namespace PsdUtilities.RazorPdf;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddRazorPdfConverter(this IServiceCollection services) => AddRazorPdfConverter(services, BrowserType.Chromium);
    public static IServiceCollection AddRazorPdfConverter(this IServiceCollection services, params string[] initializeBrowserTypes)
    {
        services.AddSingleton<IPlaywrightService, PlaywrightService>();
        services.AddHostedService<PlaywrightInitializerHostedService>(provider =>
            new(provider.GetRequiredService<IPlaywrightService>(), initializeBrowserTypes)
        );

        services.AddTransient<IRazorPdfConverter, RazorPdfConverter>();
        services.AddLogging();

        return services;
    }

    public class PlaywrightInitializerHostedService : IHostedService
    {
        private readonly IPlaywrightService _playwright;
        private readonly string[] _initializeBrowserTypes;

        public PlaywrightInitializerHostedService(IPlaywrightService playwright, string[] initializeBrowserTypes)
        {
            _playwright = playwright;
            _initializeBrowserTypes = initializeBrowserTypes;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _playwright.InitializePlaywrightAsync();

            foreach (var browserType in _initializeBrowserTypes)
            {
                await _playwright.InitializeBrowserAsync(browserType);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _playwright.DisposeAsync();
        }
    }
}