using Microsoft.Playwright;

namespace PsdUtilities.RazorPdf.Options;

public sealed class PdfOptions : PagePdfOptions
{
    public PdfFormats FormatTemplate
    {
        internal get => PdfFormats.None;
        set => Format = value.Format;
    }

    public static readonly PdfOptions Default = new PdfOptions()
    {
        FormatTemplate = PdfFormats.A4
    };
}