using System;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
//https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-key-vault-references-dotnet-core?tabs=cmd%2Ccore3x#connect-to-key-vault

namespace AppConfigKeyVault
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string configUrl;
                try
                {
                    configUrl = args[0].Trim();
                }
                catch 
                {
                    Console.WriteLine("Error parsing command line arguments, expected Azure App Configuration url parameter");
                    throw;
                }

                /*
                 *  Environment variables
                    set AZURE_CLIENT_ID = b421b443-1669-4cd7-b5b1-394d5c945002
                    set AZURE_CLIENT_SECRET = -some-secret-
                    set AZURE_TENANT_ID = b421b443-1669-4cd7-b5b1-394d5c945002
                */
                var credentials = new DefaultAzureCredential(true);

                var builder = new ConfigurationBuilder();
                builder.AddAzureAppConfiguration(options =>
                    {
                        options
                            .Connect(configUrl)
                            .ConfigureKeyVault(
                                kv => kv.SetCredential(credentials));
                    });
                var config = builder.Build();

                // mykey is a secret in KeyVault, secret is referenced from App Configuration
                Console.WriteLine($"mykey: {config["mykey"]}");
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
