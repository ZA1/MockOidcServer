using System.Text;

namespace MockOidcServer.Configurations;

public static class ConfigurationExtensions
{
    public static void AddJsonConfig(this ConfigurationManager configuration, string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        configuration.AddJsonStream(stream);
    }
}