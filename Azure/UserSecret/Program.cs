using System;
using Microsoft.Extensions.Configuration;

namespace UserSecret
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<AppSecrets>()
                .Build();
            var appSecrets = new AppSecrets();

            config.GetSection(nameof(AppSecrets)).Bind(appSecrets);

            Console.WriteLine($"SecretKeyA: {appSecrets.SecretKeyA}");
            Console.WriteLine($"SecretKeyB: {appSecrets.SecretKeyB}");

            //IOptions<AppSecrets> myOptions = Options.Create(AppSecrets);

            Console.ReadKey(false);
        }
    }

    public class AppSecrets
    {
        public string SecretKeyA { get; set; }
        public string SecretKeyB { get; set; }
    }
}
