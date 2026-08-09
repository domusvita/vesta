using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace Vesta.UnitTests.TestHelpers;

/// <summary>
/// Minimal fake of the abstract <see cref="FunctionContext"/> so tests can populate
/// <see cref="Items"/> (e.g. with a ClaimsPrincipal under the "User" key) without needing
/// a real Azure Functions host.
/// </summary>
public class FakeFunctionContext : FunctionContext
{
    public FakeFunctionContext()
    {
        Items = new Dictionary<object, object>();
        InstanceServices = new FakeServiceProvider();
    }

    public override string InvocationId { get; } = Guid.NewGuid().ToString();

    public override string FunctionId { get; } = "test-function-id";

    public override TraceContext TraceContext { get; } = null!;

    public override BindingContext BindingContext { get; } = null!;

    public override RetryContext RetryContext { get; } = null!;

    public override IServiceProvider InstanceServices { get; set; }

    public override FunctionDefinition FunctionDefinition { get; } = null!;

    public override IDictionary<object, object> Items { get; set; }

    public override IInvocationFeatures Features { get; } = null!;

    private sealed class FakeServiceProvider : IServiceProvider
    {
        private readonly IOptions<WorkerOptions> _workerOptions =
            Options.Create(new WorkerOptions { Serializer = new JsonObjectSerializer() });

        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IOptions<WorkerOptions>) ? _workerOptions : null;
        }
    }
}
