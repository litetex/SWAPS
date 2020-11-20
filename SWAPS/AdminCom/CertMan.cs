using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SWAPS.AdminCom
{
   public static class CertMan
   {
      public static X509Certificate2 CreateSelfSignedCertInMemory(string password = "WeNeedASaf3rPassword", string certName = "localhostSelfSigned")
      {
         var sanBuilder = new SubjectAlternativeNameBuilder();
         sanBuilder.AddIpAddress(IPAddress.Loopback);
         sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
         sanBuilder.AddDnsName("localhost");
         sanBuilder.AddDnsName(Environment.MachineName);

         var distinguishedName = new X500DistinguishedName($"CN={certName}");

         using var rsa = RSA.Create(2048);

         var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

         request.CertificateExtensions.Add(
             new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

         request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

         request.CertificateExtensions.Add(sanBuilder.Build());

         var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
         certificate.FriendlyName = certName;

         return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password);
      }
   }
}
