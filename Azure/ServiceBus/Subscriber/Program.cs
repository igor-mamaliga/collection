using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber
{
    class Program
    {
        const string ServiceBusConnectionString = @"Endpoint=sb://service-bus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=**********************************";
        const string TopicName = "service-bus-topic";
        static ISubscriptionClient subscriptionClient;
        const string SubscriptionName = "service-bus-subscr";

        static void Main(string[] args)
        {
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);
            Console.WriteLine("Press ENTER key to exit after receiving all the  messages.");
            // Configure the message handler options in terms of exception handling,  
            // number of concurrent messages to deliver, etc.           
            var messageHandlerOptions = new MessageHandlerOptions(
                ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback          
                // ProcessMessagesAsync(), set to 1 for simplicity.         
                // Set it according to how many messages the application wants to            
                // process in parallel.             
                MaxConcurrentCalls = 1,
                // Indicates whether the message pump should automatically complete         
                // the messages after returning from user callback.             
                // False below indicates the user callback handles the complete          
                // operation as in ProcessMessagesAsync().              
                AutoComplete = false   
            };
            // Register the function that processes messages.     
            subscriptionClient.RegisterMessageHandler(
                ProcessMessagesAsync,
                messageHandlerOptions);
            Console.ReadKey(false);
            subscriptionClient.CloseAsync();
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            // Complete the message so that it is not received again.          
            // This can be done only if the subscriptionClient is created in         
            // ReceiveMode.PeekLock mode (which is the default).      
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            // Note: Use the cancellationToken passed as necessary to determine if the     
            // subscriptionClient has already been closed.          
            // If subscriptionClient has already been closed, you can choose to not       
            // call CompleteAsync() or AbandonAsync() etc.          
            // to avoid unnecessary exceptions.    
        }

        // Use this handler to examine the exceptions received on the message pump.    
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception        " +
          $"    {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
