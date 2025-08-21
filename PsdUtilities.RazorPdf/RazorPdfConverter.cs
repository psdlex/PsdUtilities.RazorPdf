using System.Collections.ObjectModel;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using PsdUtilities.RazorPdf.Options;
using PsdUtilities.RazorPdf.Playwright;

namespace PsdUtilities.RazorPdf;

public sealed class RazorPdfConverter : IRazorPdfConverter
{
    private readonly HtmlRenderer _htmlRenderer;
    private readonly IPlaywrightService _playwrightService;

    public RazorPdfConverter(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IPlaywrightService playwrightService)
    {
        _htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        _playwrightService = playwrightService;
    }

    public Task<byte[]> GeneratePdfAsync<TRazor>() where TRazor : IComponent
        => GeneratePdfAsync<TRazor>(PdfOptions.Default, ReadOnlyDictionary<string, object?>.Empty);
    public Task<byte[]> GeneratePdfAsync<TRazor>(PdfOptions options) where TRazor : IComponent
        => GeneratePdfAsync<TRazor>(options, ReadOnlyDictionary<string, object?>.Empty);
    public Task<byte[]> GeneratePdfAsync<TRazor>(IDictionary<string, object?> parameters) where TRazor : IComponent
        => GeneratePdfAsync<TRazor>(PdfOptions.Default, parameters);

    public async Task<byte[]> GeneratePdfAsync<TRazor>(PdfOptions options, IDictionary<string, object?> parameters) where TRazor : IComponent
    {
        var browser = _playwrightService.GetBrowser(BrowserType.Chromium);
        var htmlString = await GetHtmlString<TRazor>(parameters);

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(htmlString);

        var bytes = await page.PdfAsync(options);
        await page.CloseAsync();

        return bytes;
    }

    private async Task<string> GetHtmlString<TRazor>(IDictionary<string, object?> dictionaryParameters) where TRazor : IComponent
    {
        return await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var parameters = ParameterView.FromDictionary(dictionaryParameters);
            var htmlComponent = await _htmlRenderer.RenderComponentAsync<TRazor>(parameters);
            return htmlComponent.ToHtmlString();
        });
    }

    public void Dispose() => _htmlRenderer.Dispose();
    public ValueTask DisposeAsync() => _htmlRenderer.DisposeAsync();
}