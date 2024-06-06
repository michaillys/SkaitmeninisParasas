using System;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


class Program
{
    static async Task Main(string[] args)
    {
        var signatureData = await GetDataFromServer();

        if (VerifySignature(signatureData.Message, signatureData.Signature, signatureData.PublicKey))
        {
            Console.WriteLine("The signature is valid.");
            Console.WriteLine("Message: " + signatureData.Message);
        }
        else
        {
            Console.WriteLine("The signature is invalid.");
        }
    }

    private static async Task<SignatureData> GetDataFromServer()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync("http://localhost:5000/api/signature");
            return JsonConvert.DeserializeObject<SignatureData>(response);
        }
    }

    public static bool VerifySignature(string message, string signature, string publicKeyString)
    {
        byte[] signatureBytes = Convert.FromBase64String(signature);
        byte[] publicKeyBytes = Convert.FromBase64String(publicKeyString);

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportCspBlob(publicKeyBytes);
            var encoder = new UTF8Encoding();
            byte[] data = encoder.GetBytes(message);
            return rsa.VerifyData(data, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);
        }
    }
}

public class SignatureData
{
    public string? PublicKey { get; set; }
    public string? Message { get; set; }
    public string? Signature { get; set; }
}
