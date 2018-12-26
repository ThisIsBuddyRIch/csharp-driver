using System;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json;
using App.Metrics.Reporting.Console;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

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
                              options =>
                              {
                                  options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                                  options.AppendMetricsToTextFile = true;
                                  options.FlushInterval = TimeSpan.FromSeconds(1);
                                  options.OutputPathAndFileName = @"C:\metrics.txt";
                              })
                          .Build();

            var task = Task.Run(() =>
            {
                while (true)
                {
                    var tasks = metrics.ReportRunner.RunAllAsync();

                    Task.WaitAll(tasks.ToArray());
                    Thread.Sleep(50);
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



            var session = cluster.Connect("driver_test");

            var table = new Table<Person>(session);
            table.CreateIfNotExists();

            
            Parallel.ForEach(Enumerable.Range(0, 1000), (i, state) => table.Execute());
        }

        private static Person CreatePerson()
        {
            return new Person
            {
                UserId = Guid.NewGuid(),
                Name = "Kirill",
                Age = 10
            };
        }
    }
}