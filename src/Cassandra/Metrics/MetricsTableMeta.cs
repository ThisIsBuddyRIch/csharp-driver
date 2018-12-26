using Cassandra.Data.Linq;

namespace Cassandra.Metrics
{
    public class MetricsTableMeta
    {
        public MetricsTableMeta(ITable table)
        {
            Keyspace = table.KeyspaceName;
            TableName = table.Name;
        }
        
        public string Keyspace { get; set; }

        public string TableName { get; set; }
    }
}