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
        private const string EndpointUri = "https://contoso-ready.documents.azure.com:443/";
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

            client = new DocumentClient(new Uri(EndpointUri), AuthorizationKey, connectionPolicy: connectionPolicy);
            client.OpenAsync().ConfigureAwait(false);

            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();

                FeedOptions feedOptions = new FeedOptions
                {
                    EnableCrossPartitionQuery = true
                };
                // Read most recent document 
                var query = "SELECT TOP 1 * FROM c WHERE c.EventName = 'Check_engine_light' ORDER BY c._ts desc";
                //var document = client.CreateDocumentCollectionQuery(UriFactory.CreateDocumentCollectionUri("Demo", "VehicleData"), query, feedOptions).ToList().FirstOrDefault();
                var document = client.CreateDocumentQuery<dynamic>(UriFactory.CreateDocumentCollectionUri("Demo", "VehicleData"), feedOptions, query).ToList().FirstOrDefault();

                sw.Stop();

                Console.WriteLine($"Read document in {sw.ElapsedMilliseconds} ms from {client.ReadEndpoint}");

                Thread.Sleep(1000);
            }
        }
    }
}
