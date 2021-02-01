using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading;

namespace QueueStorage
{
    class Program
    {
        private const string connectionString = "https://********.queue.core.windows.net/my-queue";
        private const int MAX_MESSAGES_COUNT = 0;

        static void Main(string[] args)
        {
            var credential = new ClientSecretCredential("********.onmicrosoft.com", "********", "********");

            Uri uri = new Uri(connectionString);
           
            QueueClient queueClient = new QueueClient(uri, credential);
            //Create the queue queueClient.CreateIfNotExists();    
            //Sending messages to the queue.         
            for (int i = 0; i < MAX_MESSAGES_COUNT; i++)
            {
                var message = $"Message {i} {DateTime.Now}";
                // BinaryData binaryData=new BinaryData()
                queueClient.SendMessageAsync(message).Wait();
                Console.WriteLine(message);
                Thread.Sleep(100);
            }

            //Getting the length of the queue           
            QueueProperties queueProperties = queueClient.GetProperties();
            int? cachedMessageCount = queueProperties.ApproximateMessagesCount;
            //Reading messages from the queue without removing the message    
            Console.WriteLine("Reading message from the queue without removing them from the queue");
            PeekedMessage[] peekedMessages = queueClient.PeekMessages((int)cachedMessageCount);

            foreach (PeekedMessage peekedMessage in peekedMessages)
            {
                Console.WriteLine($"Message read from the queue: {peekedMessage.MessageText}");
                //Getting the length of the queue              
                queueProperties = queueClient.GetProperties();
                int? queueLenght = queueProperties.ApproximateMessagesCount;
                Console.WriteLine($"Current lenght of the queue {queueLenght}");
            }

            //Reading messages removing it from the queue        
            Console.WriteLine("Reading message from the queue removing");
            QueueMessage[] messages = queueClient.ReceiveMessages((int)cachedMessageCount);
            foreach (QueueMessage message in messages)
            {
                Console.WriteLine($"Message read from the queue: {message.MessageText}");
                //You need to process the message in less than 30 seconds.          
                //queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                //Getting the length of the queue             
                queueProperties = queueClient.GetProperties();
                int? queueLenght = queueProperties.ApproximateMessagesCount;
                Console.WriteLine($"Current lenght of the queue {queueLenght}");

                var response = queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            }
            Console.ReadKey(false);
        }
    }
}
