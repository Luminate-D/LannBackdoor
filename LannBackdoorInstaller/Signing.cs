using System.Security.Cryptography;
using System.Text;
using LannConstants;

namespace SystemModule;

public static class Signing {
    public static bool VerifySigned(string source, string signatureBase64) {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(Constants.RsaPubKey);

        byte[] bytesSource = Encoding.UTF8.GetBytes(source);
        byte[] bytesSignature = Convert.FromBase64String(signatureBase64);

        return rsa.VerifyData(bytesSource, bytesSignature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
    }
}