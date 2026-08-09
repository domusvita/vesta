using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Vesta.UnitTests.TestHelpers;

/// <summary>
/// Minimal fake of the abstract <see cref="HttpResponseData"/> so function output (status code,
/// body) can be asserted against in tests without a real Azure Functions host.
/// </summary>
public class FakeHttpResponseData : HttpResponseData
{
    public FakeHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
    }

    public override HttpStatusCode StatusCode { get; set; }

    public override HttpHeadersCollection Headers { get; set; }

    public override Stream Body { get; set; }

    public override HttpCookies Cookies { get; } = null!;

    public string ReadBodyAsString()
    {
        Body.Position = 0;
        using var reader = new StreamReader(Body, leaveOpen: true);
        var text = reader.ReadToEnd();
        Body.Position = 0;
        return text;
    }
}
