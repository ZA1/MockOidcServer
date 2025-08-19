using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MockOidcServer.Certificates;

public static class CertificateGenerator
{
    static CertificateGenerator()
    {
        SigningCert = CreateCert("Signing");
        HttpsCert = CreateCert("Https");
    }

    private static X509Certificate2 CreateCert(string name)
    {
        // Generate new certificate
        var request = new CertificateRequest("cn=MockAuth", RSA.Create(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        var cert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10));
        return cert;
    }

    public static X509Certificate2 SigningCert { get; }
    public static X509Certificate2 HttpsCert { get; }
}