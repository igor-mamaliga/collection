using System;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Publisher
{
    class Program
    {
        const string ServiceBusConnectionString = @"Endpoint=sb://service-bus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=***********************************************";
        const string TopicName = "service-bus-topic";
        const int numberOfMessagesToSend = 10;

        static ITopicClient topicClient; 
        
        static void Main(string[] args)
        {
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine();
            // Send messages.           
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {                    
                    // Create a new message to send to the topic.                   
                    string messageBody = $"Message {i} {DateTime.Now}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    // Write the body of the message to the console.                 
                    Console.WriteLine($"Sending message: {messageBody}");
                    // Send the message to the topic.                
                    topicClient.SendAsync(message).Wait();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
            Console.ReadKey();
            topicClient.CloseAsync();
        }
    }

}
