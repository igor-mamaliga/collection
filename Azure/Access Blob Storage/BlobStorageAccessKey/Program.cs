using System;
using System.IO;
using System.Net;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using static System.Console;

namespace BlobStorageAccessKey
{
    public class BlobParameters
    {
        public string AccountName { get; init; }
        public string AccountKey { get; init; }
        public string ContainerName { get; init; }
        public static string Protocol { get { return "DefaultEndpointsProtocol=https;"; } }
    }

    class Program
    {
        readonly static string nl = Environment.NewLine;
        readonly static Encoding ASCII = Encoding.ASCII;

        static BlobContainerClient GetContainer(
            BlobParameters @params)
        {
            var name = $"AccountName={@params.AccountName};";
            var key = $"AccountKey={@params.AccountKey}";

            string connectionString = $"{BlobParameters.Protocol}{name}{key}";

            var blobServiceClient =
                new BlobServiceClient(connectionString);
            BlobContainerClient container =
                blobServiceClient.GetBlobContainerClient(@params.ContainerName);
            container.CreateIfNotExists(PublicAccessType.BlobContainer);

            return container;
        }

        static void CreateBlob(BlobClient blobClient, string text)
        {
            var content = ASCII.GetBytes(text);
            using var memStream = new MemoryStream();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var response = blobClient.Upload(memStream).GetRawResponse();
            var created = response.Status == (int)HttpStatusCode.Created;
            if (created)
                WriteLine($"Blob \"{blobClient.Name}\" was created");
            else
                throw new Exception($"Error creating blob.");
        }

        static void UpdateBlob(BlobClient blobClient, string appendText)
        {
            var oldText = ReadBlob(blobClient);
            var newText = oldText + appendText;
            var content = ASCII.GetBytes(newText);
            // create a Stream with new content
            using var memStream = new MemoryStream();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            // upload new content to blob
            var response = blobClient.Upload(memStream, true).GetRawResponse();
            var updated = response.Status == (int)HttpStatusCode.Created;

            if (updated)
            {
                WriteLine($"Blob was updated");
                WriteLine($"New blob content:{nl}{ReadBlob(blobClient)}");
            }
            else
                throw new Exception("Error updating blob");
        }

        static string ReadBlob(BlobClient blobClient)
        {
            // read blob content
            using var memStream = new MemoryStream();
            blobClient.DownloadTo(memStream);
            var content = ASCII.GetString(memStream.ToArray());
            return content;
        }

        static void DeleteBlob(BlobContainerClient containerClient, BlobClient blobClient)
        {
            var deleted = containerClient.DeleteBlobIfExists(blobClient.Name).Value;
            if (deleted)
                WriteLine("Blob was deleted");
            else
                throw new Exception("Error deleting blob");
        }

        static void Main(string[] args)
        {
            var @params = new BlobParameters
            {
                AccountName = "storagexj3f9",
                AccountKey = "*********************************************************************==",
                ContainerName = "my-container"
            };

            // compose text for blob content
            var text = $"Hello blob{nl}" + 
                $"Current time: {DateTime.Now:HH:mm:ss.fff}";

            var containerClient = GetContainer(@params);
            var blobName = $"my-blob-{Guid.NewGuid():D}.txt";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
                                
            CreateBlob(blobClient, text);
            
            // read blob content
            var content = ReadBlob(blobClient);
            WriteLine($"{nl}Blob content:{nl}{content}{nl}");

            // update blob
            var updatedOn = $"{nl}" +
                $"  Updated on: {DateTime.Now:HH:mm:ss.fff}";
            UpdateBlob(blobClient, updatedOn);

            WriteLine("Press 'D' key to delete blob or press any other key to exit");
            if (ReadKey(true).Key == ConsoleKey.D)
            {
                DeleteBlob(containerClient, blobClient);
                
                WriteLine("Press any key to exit");
                ReadKey(false);
            }
        }
    }
}
