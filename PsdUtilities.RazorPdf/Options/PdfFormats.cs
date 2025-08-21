namespace PsdUtilities.RazorPdf.Options;

public sealed class PdfFormats
{
    private PdfFormats(string format)
    {
        Format = format;
    }

    public string Format { get; }

    public static readonly PdfFormats None = new(string.Empty);
    public static readonly PdfFormats A0 = new(nameof(A0));
    public static readonly PdfFormats A1 = new(nameof(A1));
    public static readonly PdfFormats A2 = new(nameof(A2));
    public static readonly PdfFormats A3 = new(nameof(A3));
    public static readonly PdfFormats A4 = new(nameof(A4));
    public static readonly PdfFormats A5 = new(nameof(A5));
    public static readonly PdfFormats A6 = new(nameof(A6));
}