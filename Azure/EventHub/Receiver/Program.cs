using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading;

namespace Receiver
{
    internal class Program
    {
        private const string EventHubConnectionString =
            @"Endpoint=sb://event-hub-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=*******************";
        private const string EventHubName = "event-hub-432423";
        private const string StorageContainerName = "container";
        private static readonly string StorageConnectionString =
            @"DefaultEndpointsProtocol=https;AccountName*********;AccountKey=******;EndpointSuffix=core.windows.net";

        static void Main(string[] args)
        {
            List<EventProcessorHost> eventProcessorHosts = new List<EventProcessorHost>();
                var eventProcessorHost = new EventProcessorHost(
                    EventHubName,
                    PartitionReceiver.DefaultConsumerGroupName,
                    EventHubConnectionString,
                    StorageConnectionString,
                StorageContainerName);
                eventProcessorHosts.Add(eventProcessorHost);
            eventProcessorHosts[0].RegisterEventProcessorAsync<SimpleEventProcessor>();
            
            Console.WriteLine("Receiving.Press ENTER to stop worker.");
            Console.ReadKey(false);
            eventProcessorHosts.ForEach(h =>
            {
                // Disposes of the Event Processor Host        
                h.UnregisterEventProcessorAsync();
            });
        }
    }

    public class SimpleEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition' {context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}' thread:{Thread.CurrentThread.ManagedThreadId}");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error:            {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                                                                                                                     
                Console.WriteLine($"messages count: {messages.Count()} Message received. Partition: '{context.PartitionId}', Data: '{data}',  thread:{Thread.CurrentThread.ManagedThreadId}");
            }
            //     Writes the current offset and sequenceNumber to the checkpoint store via the
            //     checkpoint manager.
            return context.CheckpointAsync();
        }
    }
}
