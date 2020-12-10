using System;
using StackExchange.Redis;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace RedisCache
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string redisCacheConnString;
               
                try
                {
                    redisCacheConnString =  args[0].Trim();
                }
                catch
                {
                    Console.WriteLine("Error parsing command line arguments, \r\n" +
                        "expected [0]Redis Cache connection string, example:" +
                        "your-name.redis.cache.windows.net:6380,password=*****************,ssl=True,abortConnect=False");
                    Console.WriteLine("Trying to read RedisConnectionString from user secrets..");
                    try
                    {
                        var builder = new ConfigurationBuilder();
                        builder.AddUserSecrets<SecretStuff>();
                        var config = builder.Build();
                        redisCacheConnString = config["SecretStuff:RedisConnectionString"];
                        Console.WriteLine($"RedisConnectionString = {"".PadLeft(redisCacheConnString.Length, '*')}");
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }

                var lazyConnection = new Lazy<ConnectionMultiplexer>(() => 
                { 
                    return ConnectionMultiplexer.Connect(redisCacheConnString); 
                });
                IDatabase cache = lazyConnection.Value.GetDatabase();
                var command = "PING";
                var cmdResult = cache.Execute(command);
                Console.WriteLine($"{command} command result: {cmdResult}");

                var key = "Message";
                if (cache.KeyExists(key))
                {
                    //cache.KeyDelete(key);
                }
                var message = "Hello Redis Cache!";
                //cache.StringSet(key, message, TimeSpan.FromHours(10));
                string value = cache.StringGet(key);
                Debug.Assert(value == message);
                Console.WriteLine($"From Redis Cache: {value}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex);
            }
            Console.WriteLine("Press any key..");
            Console.ReadKey();
        }
    }

    public class SecretStuff
    {
        public string RedisConnectionString { get; set; }
    }
}
