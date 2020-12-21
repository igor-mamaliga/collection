using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace EventHubsSender
{
    class Program
    {
        static EventHubClient eventHubClient;
        const string EventHubConnectionString = @"Endpoint=sb://event-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=*******************";
        const string EventHubName = "event-hub";
        const int numMessagesToSend = 8000;
        const int sleepMilliseconds = 2000;


        static void Main(string[] args)
        {
            var id = Guid.NewGuid().ToString().Substring(12, 4);
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}, sender id:[{id}] {DateTime.Now.ToLongTimeString()}";
                    List<EventData> e = new List<EventData>();
                    var data = new EventData(Encoding.UTF8.GetBytes(message));
                    e.Add(data);
                    eventHubClient.SendAsync(e).Wait();
                    Console.WriteLine($"Send: {message}");
                    if (sleepMilliseconds > 0)
                        Thread.Sleep(sleepMilliseconds);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
            eventHubClient.CloseAsync();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(false);
        }
    }
}
