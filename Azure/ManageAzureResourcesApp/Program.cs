// Author: Igor M.
// year: 2020
using System;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using OperatingSystem = Microsoft.Azure.Management.AppService.Fluent.OperatingSystem;

namespace SDB.ManageAzureResourcesApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Create Microsoft Azure resources";

            // login
            var credentials = 
                SdkContext
               .AzureCredentialsFactory
               .FromFile("credentials.txt");

            Console.WriteLine("Connecting..");
            var azure =
                Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

            var region = Region.EuropeWest;
            var groupName = "SDB-resource-group";
            var appServicePlanName = "SDB-app-service-plan";
            var webAppName = "SDB-web-app";
            var funcAppName = "SDB-func-app";
            var storageAccName = "sdbstorage92097485130234";

            // create Resource Group
            Console.WriteLine($"Creating \"{groupName}\"..");
            var resourceGroup =
                azure
                .ResourceGroups
                .Define(groupName)
                .WithRegion(region)
                .Create();

            // create storage account
            Console.WriteLine($"Creating storage \"{storageAccName}\"..");
            var storageAccount =
                azure
                .StorageAccounts
                .Define(storageAccName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroup)
                .Create();

            // create App Service Plan
            Console.WriteLine($"Creating \"{appServicePlanName}\"..");
            var appServicePlan = 
                azure
                .AppServices
                .AppServicePlans
                .Define(appServicePlanName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroup)
                .WithPricingTier(PricingTier.FreeF1)
                .WithOperatingSystem(OperatingSystem.Windows)
                .WithCapacity(1)
                .Create();

            // create WebApp
            Console.WriteLine($"Creating \"{webAppName}\"..");
            var webApp = 
                azure
                .AppServices
                .WebApps
                .Define(webAppName)
                .WithExistingWindowsPlan(appServicePlan)
                .WithExistingResourceGroup(resourceGroup)
                .Create();

            // create FuncApp
            Console.WriteLine($"Creating \"{funcAppName}\"..");
            var funcApp =
                azure
                .AppServices
                .FunctionApps
                .Define(funcAppName)
                .WithExistingAppServicePlan(appServicePlan)
                .WithExistingResourceGroup(resourceGroup)
                .WithExistingStorageAccount(storageAccount)
                .Create();
                
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            // delete all previously created resources
            //azure.ResourceGroups.DeleteByName(groupName);
        }
    }
}
