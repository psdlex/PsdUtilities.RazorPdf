namespace PsdUtilities.RazorPdf;

public static class RazorPdfUtility
{
    public static void InstallRequiredDependencies()
    {
        Microsoft.Playwright.Program.Main(["Install"]);
    }
}
