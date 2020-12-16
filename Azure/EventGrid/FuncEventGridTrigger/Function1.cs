using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FuncEventGridTrigger 
{  
    
    public static class Function1 
    {
        [FunctionName("EventGridTrigger")] 
        public static void Run(
            [EventGridTrigger] EventGridEvent eventGrid, 
            ILogger log)
        {
            log.LogInformation("C# Event Grid trriger handling EventGrid Events."); 
            log.LogInformation($"New event received: {eventGrid.Data}"); 

            if (eventGrid.Data is StorageBlobCreatedEventData) 
            { 
                var eventData = (StorageBlobCreatedEventData)eventGrid.Data;
                log.LogInformation($"Got BlobCreated event data, blob URI {eventData.Url}"); 
            } 
            else if (eventGrid.EventType.Equals("MyCompany.Items.NewItemCreated"))
            { 
                NewItemCreatedEventData eventData = ((JObject)eventGrid.Data).ToObject<NewItemCreatedEventData>();
                log.LogInformation($"New Item Custom Event, Name {eventData.itemName}{Environment.NewLine}" +
                    $"Topic: {eventGrid.Topic}{Environment.NewLine}"+
                    $"Subject: {eventGrid.Subject}{Environment.NewLine}"+
                    $"DataVersion: {eventGrid.DataVersion}{Environment.NewLine}");
            } 
        }
    }

    class NewItemCreatedEventData {[JsonProperty(PropertyName = "name")] public string itemName; }
}