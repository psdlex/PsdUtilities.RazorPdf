
using Microsoft.AspNetCore.Components;

using PsdUtilities.RazorPdf.Options;

namespace PsdUtilities.RazorPdf;
public interface IRazorPdfConverter : IDisposable, IAsyncDisposable
{
    Task<byte[]> GeneratePdfAsync<TRazor>() where TRazor : IComponent;
    Task<byte[]> GeneratePdfAsync<TRazor>(IDictionary<string, object?> parameters) where TRazor : IComponent;
    Task<byte[]> GeneratePdfAsync<TRazor>(PdfOptions options) where TRazor : IComponent;
    Task<byte[]> GeneratePdfAsync<TRazor>(PdfOptions options, IDictionary<string, object?> parameters) where TRazor : IComponent;
}