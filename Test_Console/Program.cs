using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Owin.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Test_Console;
public class Program
{
    static void Main()
    {
        string baseAddress = "http://localhost:9000/";

        // Start OWIN host 
        using (WebApp.Start<Startup>(url: baseAddress))
        {
            // Create HttpClient and make a request to api/values 
            HttpClient client = new HttpClient();

            var response = client.GetAsync(baseAddress + "api/values").Result;

            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            Console.ReadLine();
        }
    }
}