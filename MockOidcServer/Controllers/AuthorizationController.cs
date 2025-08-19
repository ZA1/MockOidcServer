using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MockOidcServer.Certificates;
using MockOidcServer.Configurations;

namespace MockOidcServer.Controllers;

public class AuthorizationController : Controller
{
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> Authorize(
        [FromServices] IOptions<UsersOptions> users,
        [FromQuery(Name = "redirect_uri")] string redirectUri,
        [FromQuery(Name = "state")] string state,
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "nonce")] string? nonce,
        [FromQuery] string email)
    {
    
        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = users.Value.FirstOrDefault(u => u["email"] == email);
            if (user is null)
            {
                return BadRequest();
            }

            var scheme = Request.Scheme;
            var host = Request.Host;
            var baseUri = new Uri($"{scheme}://{host}/");
            var code = Base64UrlEncoder.Encode(JsonSerializer.Serialize(new Code
            {
                Issuer = baseUri.ToString(),
                Audience = clientId,
                Claims = user,
                Nonce = nonce
            }));
            var uri = new Uri(redirectUri);
            var query = QueryString.Create(new Dictionary<string, string?>
            {
                { "code", code },
                { "state", state }
            });
            redirectUri = uri.AbsoluteUri + query;
            return Redirect(redirectUri);
        }

        return View(users.Value);
    }

    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange(
        [FromServices] IOptions<AppSettings> appSettings,
        string code)
    {
        var codeValues = JsonSerializer.Deserialize<Code>(Base64UrlEncoder.Decode(code));

        if (!codeValues.Claims.ContainsKey("sub"))
        {
            codeValues.Claims.Add("sub", codeValues.Claims["email"]);
        }

        if (!string.IsNullOrWhiteSpace(codeValues.Nonce))
        {
            codeValues.Claims.Add("nonce", codeValues.Nonce);
        }

        var claims = codeValues.Claims.Select(c => new Claim(c.Key, c.Value));

        var accessToken = CreateJwt(codeValues, claims
            .Where(c => appSettings.Value.AccessTokenClaims.Contains(c.Type)));
        var idToken = CreateJwt(codeValues, claims);
        return Ok(new
        {
            access_token = accessToken,
            expires_in = 3600,
            token_type = "Bearer",
            id_token = idToken
        });
    }
    
    [HttpPost("~/connect/logout")]
    [HttpGet("~/connect/logout")]
    public async Task<IActionResult> Logout(
        [FromQuery(Name = "post_logout_redirect_uri")]
        string redirectUri,
        [FromQuery(Name = "state")] string state)
    {
        if (redirectUri is not null)
        {
            var query = QueryString.Create(new Dictionary<string, string?>
            {
                { "state", state }
            });
            return Redirect(redirectUri + query);
        }

        return View();
    }

    private static string CreateJwt(Code codeValues, IEnumerable<Claim> claims)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = codeValues.Issuer,
            Audience = codeValues.Audience,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new X509SecurityKey(CertificateGenerator.SigningCert),
                SecurityAlgorithms.RsaSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var jwt = new JsonWebTokenHandler().CreateToken(descriptor);
        return jwt;
    }

    private class Code
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required Dictionary<string, string> Claims { get; init; }
        public required string? Nonce { get; init; }
    }
}