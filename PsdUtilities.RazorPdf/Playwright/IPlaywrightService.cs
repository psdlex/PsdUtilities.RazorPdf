using Microsoft.Playwright;

namespace PsdUtilities.RazorPdf.Playwright;

public interface IPlaywrightService : IAsyncDisposable
{
    IPlaywright Playwright { get; }
    IBrowser GetBrowser(string browserType);
    Task InitializePlaywrightAsync();
    Task<IBrowser> InitializeBrowserAsync(string browserType);
}
