using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MockOidcServer.Configurations;

public class UsersOptions: List<Dictionary<string, string>>
{
    public const string SectionName = "Users";
}

public class AppSettings
{
    public string[] AccessTokenClaims { get; set; } = ["sub", "roles"];
}