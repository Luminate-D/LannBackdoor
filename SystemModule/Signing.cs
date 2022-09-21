using System.Security.Cryptography;
using System.Text;

namespace SystemModule;

public class Signing {
    private const string RsaPubkey = @"-----BEGIN PUBLIC KEY-----MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3B8JBWPXz9a48TVOFtveIvuYjncRN6IN3RU4syZkECv93WiCC0y4b2DgtO7NODG3g/H4Srf+SZw30FjrWHA3QuBtzxqhGEtNKBeLiyIZiIoTk5fdVX4xa+trgvjh7Fc5X+DiC/iBfEMFmQi+rVtI3OQKMC1ChUv3o4QXIIiidXZFJUFzjtejOMS2bxgKCcqGzRLXscP+Bb/l7DTfHD2aDvCtuhNbOs/lFN5PGmYYu7EoGj3IXcQjXxaJqdznJEvPJdJgZTrpO/oxi8meVjYfE4jP5YRMjPz8O4aIo1aMPNMWX66TYbk0b/wKIq3+1kU1KCLXezbD2pSIheWGN6OprQIDAQAB-----END PUBLIC KEY-----";

    public static bool VerifySigned(string source, string signature) {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(RsaPubkey);

        byte[] bytesSource = Encoding.UTF8.GetBytes(source);
        byte[] bytesSignature = Encoding.UTF8.GetBytes(signature);
        
        return rsa.VerifyData(bytesSource, bytesSignature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
    }
}