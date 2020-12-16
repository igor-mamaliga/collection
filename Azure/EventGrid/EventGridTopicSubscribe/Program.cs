using System;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventGridTopicSubscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddUserSecrets<EventGridSecrets>().Build();
            var secrets = new EventGridSecrets();
            config.GetSection(nameof(EventGridSecrets)).Bind(secrets);

            //IOptions<EventGridSecrets> myOptions = Options.Create(secrets);

            string topicEndpoint = secrets.TopicEndpoint;
            string apiKey = secrets.AccessKey;
            string topicHostname = new Uri(topicEndpoint).Host; 

            TopicCredentials topicCredentials = new TopicCredentials(apiKey); 
            EventGridClient client = new EventGridClient(topicCredentials);
            List<EventGridEvent> events = new List<EventGridEvent>();

            events.Add(new EventGridEvent() 
            { 
                Id = Guid.NewGuid().ToString(), 
                EventType = "MyCompany.Items.NewItemCreated", 
                Data = new NewItemCreatedEvent() { itemName = "-some-data-" },
                EventTime = DateTime.Now, 
                Subject = "Some Subject", 
                DataVersion = "1.0" 
            }); 

            client.PublishEventsAsync(topicHostname, events).GetAwaiter().GetResult(); 

            Console.WriteLine("Event published to the Event Grid Topic");
            Console.ReadKey();
        }
    }

    public class EventGridSecrets
    {
        public string AccessKey { get; set; }

        public string TopicEndpoint { get; set; }
    }

    class NewItemCreatedEvent
    {
        [JsonProperty(PropertyName = "name")]
        public string itemName;
    }
}
