using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using System.Threading;

namespace ReadWriteKeyVault
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string keyVaultName;
                string connectionString;
                try
                {
                    keyVaultName = args[0].Trim();
                    connectionString = args[1].Trim();
                }
                catch 
                {
                    Console.WriteLine("Error parsing command line arguments, \r\n" +
                        "expected [0]KeyVault name parameter, " +
                        "[1]Connection string for AzureServiceTokenProvider\r\n"+
                        "conn string RunAs=App;AppId=2ad1fe1f-d93f-4973-a1e5-e7019638b90a;TenantId=your-tenant-here.onmicrosoft.com;AppKey=your-app-secret-here");
                    throw;
                }

                var suffix = "-" + Guid.NewGuid().ToString().Substring(12, 4);
                Console.WriteLine($"Prefix: {suffix}");

                string vaultBaseURL = $"https://{keyVaultName}.vault.azure.net";
                Console.WriteLine($"Vault url: {vaultBaseURL}");
                //Get a token for accessing the Key Vault.            
                var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);            
                //Create a Key Vault client for accessing the items in the vault;            
                var keyVault = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                // Manage secrets in the Key Vault.            

                // Create new secret            
                Console.WriteLine("Creating a secret..");
                var secretId = "my-secret" + suffix;
                keyVault.SetSecretAsync(vaultBaseURL, secretId, "-some-secret-value-").Wait();
                var getSecretUrl = $"{vaultBaseURL}/secrets/{secretId}";
                Console.WriteLine($"Get secret value: {keyVault.GetSecretAsync(getSecretUrl).Result}");
                keyVault.DeleteSecretAsync(vaultBaseURL, secretId).Wait();
                Console.WriteLine("Secret was deleted");

                // Create new certificate            
                var certName = "TEST-KEYVAULT-CERTIFICATE" + suffix;
                Console.WriteLine("Creating a certificate..");
                // Create a new self-signed certificate            
                var policy = new CertificatePolicy
                {
                    IssuerParameters = new IssuerParameters
                    { Name = "Self", },
                    KeyProperties = new KeyProperties
                    {
                        Exportable = true,
                        KeySize = 2048,
                        KeyType = "RSA"
                    },
                    SecretProperties = new SecretProperties
                    { ContentType = "application/x-pkcs12" },
                    X509CertificateProperties = new X509CertificateProperties
                    { Subject = "CN=TEST-KEYVAULT-CERTIFICATE" }
                };
                keyVault.CreateCertificateAsync(vaultBaseURL, certName, policy, new CertificateAttributes { Enabled = true }).Wait();
                Thread.Sleep(12000);

                var certificate = keyVault.GetCertificateAsync(vaultBaseURL, certName).Result;
                Console.WriteLine($"Created certificate Identifier: {certificate.CertificateIdentifier.Identifier}");
                keyVault.DeleteCertificateAsync(vaultBaseURL, certName).Wait();
                Console.WriteLine("Certificate was deleted");

                Console.WriteLine("Creating a key...");
                string keyName = "key-name" + suffix; 
                NewKeyParameters keyParameters = 
                    new NewKeyParameters { Kty = "EC", CurveName = "SECP256K1", KeyOps = new[] { "sign", "verify" } }; 
                keyVault.CreateKeyAsync(vaultBaseURL, keyName, keyParameters).Wait();
                var key = keyVault.GetKeyAsync(vaultBaseURL, keyName).Result;
                Console.WriteLine($"Created key Identifier: {key.KeyIdentifier.Identifier}");
                keyVault.DeleteKeyAsync(vaultBaseURL, keyName).Wait();
                Console.WriteLine("Created key was deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Press any key..");
            Console.ReadKey();
        }
    }
}
