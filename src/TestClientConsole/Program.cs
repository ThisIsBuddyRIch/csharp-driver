using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json;
using App.Metrics.Reporting.Console;
using Cassandra;

namespace TestClientConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var metrics = new MetricsBuilder()
                          .Report.ToConsole(
                              options =>
                              {
                                  options.FlushInterval = TimeSpan.FromSeconds(1);
                                  options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                              }).Report.ToTextFile(
                              options => {
                                  options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                                  options.AppendMetricsToTextFile = true;
                                  options.FlushInterval = TimeSpan.FromSeconds(20);
                                  options.OutputPathAndFileName = @"C:\metrics.txt";
                              })
                          .Build();

            var task =  Task.Run(() =>
            {
                while (true)
                {
                    var tasks = metrics.ReportRunner.RunAllAsync();
                 
                    Task.WaitAll(tasks.ToArray());
                    Thread.Sleep(5000);
                }
            
            });
            
        
            UseCassandra(metrics);


            task.Wait();
        }

        private static void UseCassandra(IMetricsRoot metrics)
        {
            var cluster = Cluster.Builder()
                                 .AddContactPoints("127.0.0.1")
                                 .WithPort(9042)
                                 .WithMetrics(metrics)
                                 .Build();

            // Connect to the nodes using a keyspace
            var session = cluster.Connect("system_distributed");

            // Get name of a Cluster
            Console.WriteLine("The cluster's name is: " + cluster.Metadata.ClusterName);

            // Execute a query on a connection synchronously

            session.Execute("use \"EDICoreKeyspace\"");
            while (true)
            {
                var rs = session.Execute("SELECT * FROM document_circulation_bindings");
                Thread.Sleep(2000);
            }
        }
    }
}