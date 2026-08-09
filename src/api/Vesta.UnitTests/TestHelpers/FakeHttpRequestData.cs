using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Vesta.UnitTests.TestHelpers;

/// <summary>
/// Minimal fake of the abstract <see cref="HttpRequestData"/> so functions can be exercised
/// without a real Azure Functions host. Supports a JSON/string body and headers, and produces
/// a matching <see cref="FakeHttpResponseData"/> via <see cref="CreateResponse"/>.
/// </summary>
public class FakeHttpRequestData : HttpRequestData
{
    public FakeHttpRequestData(
        FunctionContext functionContext,
        string method = "GET",
        Uri? url = null,
        string? bodyJson = null
    ) : base(functionContext)
    {
        Method = method;
        Url = url ?? new Uri("https://localhost/api/test");
        Headers = new HttpHeadersCollection();
        Body = bodyJson is null
            ? new MemoryStream()
            : new MemoryStream(Encoding.UTF8.GetBytes(bodyJson));
    }

    public override Stream Body { get; }

    public override HttpHeadersCollection Headers { get; }

    public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = [];

    public override Uri Url { get; }

    public override IEnumerable<ClaimsIdentity> Identities { get; } = [];

    public override string Method { get; }

    public override HttpResponseData CreateResponse()
    {
        return new FakeHttpResponseData(FunctionContext);
    }
}
