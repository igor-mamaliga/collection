using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Text;

namespace SetBlobSystemProps
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = "my account";
            var containerName = "my container";
            var blobName = "my-new-blob.txt";

            Console.WriteLine(storageAccount);
            Uri blobEndpointUri = new Uri($"https://{storageAccount}.blob.core.windows.net");

            try
            {
                ClientSecretCredential cred =
                    new ClientSecretCredential(
                        "00000000-0000-0000-0000-000000000000",
                         "00000000-0000-0000-0000-000000000000",
                         "**************************************"
                        );
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobEndpointUri, cred);
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
                var content = Encoding.ASCII.GetBytes($"My new message: {DateTime.Now.ToLongTimeString()}");
                using (MemoryStream stream = new MemoryStream(content))
                {
                    blobClient.Upload(stream);
                }

                var blobProperties = blobClient.GetProperties();
                var props = blobProperties.Value;
                DisplaySomeProperties(props);

                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = "text/css",
                    ContentLanguage = "en-us",

                    // Populate remaining headers with 
                    // the pre-existing properties
                    CacheControl = props.CacheControl,
                    ContentDisposition = props.ContentDisposition,
                    ContentEncoding = props.ContentEncoding,
                    ContentHash = props.ContentHash
                };
                Console.WriteLine("- Set blob properties -");
                blobClient.SetHttpHeadersAsync(headers);

                blobProperties = blobClient.GetProperties();
                props = blobProperties.Value;
                DisplaySomeProperties(props);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine.PadLeft(5));
                Console.WriteLine(ex);
            }
            Console.ReadKey(false);
        }

        static void DisplaySomeProperties(BlobProperties props)
        {
            Console.WriteLine($"ContentType :{props.ContentType}");
            Console.WriteLine($"ContentLanguage :{props.ContentLanguage}");
            Console.WriteLine($"CacheControl :{props.CacheControl}");
            Console.WriteLine($"ContentDisposition :{props.ContentDisposition}");
            Console.WriteLine($"ContentEncoding :{props.ContentEncoding}");
            Console.WriteLine($"ContentHash :{Encoding.ASCII.GetString(props.ContentHash)}");
        }
    }
}
