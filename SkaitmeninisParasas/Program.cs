
using System.Security.Cryptography;
using System.Text;


class Program
{
    static async Task Main(string[] args)
    {
        // Input text
        Console.WriteLine("Enter text to sign:");
        string message = Console.ReadLine();

        // Generate public/private key pair
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                // Export the public key
                string publicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));

                // Sign the message
                var signedData = SignData(message, rsa.ExportParameters(true));

                // Convert the signature to a base64 string
                string signature = Convert.ToBase64String(signedData);

                // Send data to the second application
                await SendDataToServer(publicKey, message, signature);

                // Information about program interfaces
                Console.WriteLine("Public Key, Message, and Digital Signature have been sent to the server.");
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static byte[] SignData(string message, RSAParameters privateKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            var encoder = new UTF8Encoding();
            byte[] originalData = encoder.GetBytes(message);
            return rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA256"));
        }
    }

    private static async Task SendDataToServer(string publicKey, string message, string signature)
    {
        Console.WriteLine($"Public Key: " + publicKey + "Encrypted message: " + message + "Signature: " + signature);
        using (var client = new HttpClient())
        {
            var values = new Dictionary<string, string>
            {
                { "publicKey", publicKey },
                { "message", message },
                { "signature", signature }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://localhost:5000/api/signature", content);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
        }
    }
}
