using Microsoft.Playwright;

using Pw = Microsoft.Playwright.Playwright;

namespace PsdUtilities.RazorPdf.Playwright;

public sealed class PlaywrightService : IPlaywrightService
{
    private readonly List<IBrowser> _initializedBrowsers = [];

    public PlaywrightService()
    {
        Playwright = null!;
    }

    public IPlaywright Playwright { get; private set; }

    public IBrowser GetBrowser(string browserType)
    {
        return _initializedBrowsers.First(b => b.BrowserType.Name == browserType);
    }

    public async Task<IBrowser> InitializeBrowserAsync(string browserType)
    {
        var browserInterface = Playwright[browserType];

        if (browserInterface is null)
            throw new ArgumentException("Invalid browser type", nameof(browserType));

        var browser = await browserInterface.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = true
        });

        _initializedBrowsers.Add(browser);
        return browser;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var browser in _initializedBrowsers)
            await browser.DisposeAsync();

        Playwright.Dispose();
    }

    public async Task InitializePlaywrightAsync()
    {
        Playwright = await Pw.CreateAsync();
    }
}