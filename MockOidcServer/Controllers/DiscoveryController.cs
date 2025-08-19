using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using MockOidcServer.Certificates;

namespace MockOidcServer.Controllers;

public class DiscoveryDocument
{
    public string Issuer { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string EndSessionEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string UserInfoEndpoint { get; set; }
    public string JwksUri { get; set; }
    public string[] ScopesSupported { get; set; }
    public string[] ResponseTypesSupported { get; set; }
    public string[] SubjectTypesSupported { get; set; }
    public string[] IdTokenSigningAlgValuesSupported { get; set; }
}

public class DiscoveryController : Controller
{
    [HttpGet("~/.well-known/openid-configuration")]
    [OutputCache(Duration = 604800)]
    public async Task<IActionResult> DiscoveryDocument()
    {
        var scheme = Request.Scheme;
        var host = Request.Host;
        var baseUri = new Uri($"{scheme}://{host}/");

        var discoveryDocument = new DiscoveryDocument
        {
            Issuer = baseUri.ToString(),
            AuthorizationEndpoint = new Uri(baseUri, "/connect/authorize").ToString(),
            EndSessionEndpoint = new Uri(baseUri, "/connect/logout").ToString(),
            TokenEndpoint = new Uri(baseUri, "/connect/token").ToString(),
            UserInfoEndpoint = new Uri(baseUri, "/connect/userinfo").ToString(),
            JwksUri = new Uri(baseUri, "/.well-known/jwks").ToString(),
            ScopesSupported = ["openid", "profile", "email"],
            ResponseTypesSupported = ["code"],
            SubjectTypesSupported = ["public"],
            IdTokenSigningAlgValuesSupported = [CertificateGenerator.SigningCert.GetKeyAlgorithm()]
        };

        return Json(discoveryDocument,
            new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
    }

    [HttpGet("~/.well-known/jwks")]
    [OutputCache(Duration = 604800)]
    public async Task<IActionResult> Jwks()
    {
        var key = new RsaSecurityKey(CertificateGenerator.SigningCert.GetRSAPublicKey());
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
        return Json(new { keys = new[] { jwk } },
            new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
    }
}