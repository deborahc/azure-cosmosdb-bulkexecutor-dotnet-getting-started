using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace GlobalDemo
{
    class Program
    {
        private static readonly string EndpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static DocumentClient client;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            var connectionPolicy = new ConnectionPolicy()
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            //Setting read region selection preference
            connectionPolicy.PreferredLocations.Add(LocationNames.SouthCentralUS); // first preference
            connectionPolicy.PreferredLocations.Add(LocationNames.WestUS2); // third preference
            connectionPolicy.PreferredLocations.Add(LocationNames.SoutheastAsia); // second preference

            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey, connectionPolicy: connectionPolicy);
            client.OpenAsync().ConfigureAwait(false);

            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();

                var id = "36_9b913563-20d9-4156-8137-46b71709cc93";
                var partitionKey = id.Split('_')[0];
                RequestOptions requestOptions = new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                };

                var document = client.ReadDocumentAsync(UriFactory.CreateDocumentUri("Demo", "VehicleData", id), requestOptions).Result;

                sw.Stop();

                Console.WriteLine($"Read document in {sw.ElapsedMilliseconds} ms from {client.ReadEndpoint}");

                Thread.Sleep(1000);
            }
        }
    }
}
