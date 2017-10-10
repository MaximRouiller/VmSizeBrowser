using Microsoft.Azure.Management.Billing;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExtractPricing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Extracting Virtual Machines pricings...");

            Task.WaitAll(DoWork());
            

            //BillingManagementClient client = new BillingManagementClient(credentials, );

            if (Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.WriteLine("Debugger attached... \r\nPress ENTER to exit");
                Console.ReadLine();
            }
        }

        public static async Task DoWork()
        {
            var credentials = await UserTokenProvider.CreateCredentialsFromCache("ca958577-a94e-4b72-89b4-1f348d4864a3", "microsoft.com", "marouill@microsoft.com");
        }
    }
}
