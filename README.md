# PsdUtilities.RazorPdf

#### PsdUtilities.RazorPdf - a simple wrapper around `Razor HtmlRenderer` and `Microsoft.Playwright` library. Its designed to be used in a hosted app, due to the fact that it uses a hosted service to initialize the required components.

## Setup

Prematurely, you will have to install playwright dependencies (browser binaries), which conclude to the size of ~350MB in total. You can use the library's utility for that:
```csharp
// not a part of the application itself, just a pre-requirement. Run seperately from the app.
// .NET version matters here!
RazorPdfUtility.InstallRequiredDependencies();
```
You will see the installation progress in the console.

## How to use?

You will need to have some kind of hosted app, in my example i will use a regular `HostApplicationBuilder`, in a case of a WebAPI, use the appropriate builder.
```csharp
var builder = Host.CreateApplicationBuilder();

builder.Services.AddRazorPdfConverter(); // important !

var app = builder.Build();

await app.RunAsync();
```
You should prefer `app.RunAsync()` over the typical `app.Run()`, because as mentioned before, the library uses a hosted service which asynchronously initializes the underlying frameworks.<br><br>

After that, you can inject the `IRazorPdfConverter` into any of your services, in my example its going to be a hosted service.
```csharp
class MyHostedService : IHostedService
{
    private readonly IRazorPdfConverter _razorPdfConverter;

    public MyHostedService(IRazorPdfConverter razorPdfConverter)
    {
        _razorPdfConverter = razorPdfConverter;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

After that, you will need to actually register your service in the `DI`.
```csharp
builder.Services.AddHostedService<MyHostedService>();
```

<br><br>
Now, we can actually utilize the converter. We will need a `razor component` in order to render the html.
In my case, i will use **an imaginary invoice service for generating invoice models**
```csharp
class MyHostedService : IHostedService
{
    private readonly IRazorPdfConverter _razorPdfConverter;
    private readonly IInvoiceService _invoiceService;

    public MyHostedService(IRazorPdfConverter razorPdfConverter, IInvoiceService invoiceService)
    {
        _razorPdfConverter = razorPdfConverter;
        _invoiceService = invoiceService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var invoice = _invoiceService.GenerateInvoice();
        var parameters = new Dictionary<string, object?>()
        {
            { "Invoice", invoice }
        };

        var bytes = await _razorPdfConverter.GeneratePdfAsync<InvoiceView>(parameters);
        await File.WriteAllBytesAsync("output.pdf", bytes, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```
You can provide `PdfOptions` and **a set of parameters for your razor component** in the `GeneratePdfAsync` method.

## Example of a Razor component
Here is an example of a very simple razor component, which in my case is an invoice visualizer.
```razor
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8"/>
    <title>Invoice @Invoice.Number</title>
</head>

<body>
	<p>
		Invoice #: @Invoice.Number
		<br />
		Created: @Invoice.IssueDate.ToString("MMMM dd, yyyy")
		<br />
		Due: @Invoice.DueDate.ToString("MMMM dd, yyyy")
	</p>
</body>

</html>

@code {
    [Parameter] public Invoice Invoice { get; set; }
}
```
