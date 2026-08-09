using System.Security.Claims;

namespace Vesta.UnitTests.TestHelpers;

public static class TestPrincipalFactory
{
    public static ClaimsPrincipal WithAuth0Id(string auth0Id)
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, auth0Id)], "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal WithSubClaim(string sub)
    {
        var identity = new ClaimsIdentity([new Claim("sub", sub)], "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal WithNoIdClaim()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "someone")], "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}
