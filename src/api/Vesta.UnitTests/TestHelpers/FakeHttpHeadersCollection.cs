using Microsoft.Azure.Functions.Worker.Http;

namespace Vesta.UnitTests.TestHelpers;

/// <summary>
/// Minimal helper for building an <see cref="HttpHeadersCollection"/> populated with an Authorization
/// header or any other headers a test needs.
/// </summary>
public static class FakeHttpHeadersCollectionFactory
{
    public static HttpHeadersCollection Create(params (string Name, string Value)[] headers)
    {
        var collection = new HttpHeadersCollection();
        foreach (var (name, value) in headers)
        {
            collection.Add(name, value);
        }

        return collection;
    }
}
