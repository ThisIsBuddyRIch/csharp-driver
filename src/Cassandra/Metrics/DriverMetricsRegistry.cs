using System.Threading;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Timer;

namespace Cassandra.Metrics
{
    public static class DriverMetricsRegistry
    {
        public static TimerOptions GetRequestTimer(string keyspace, string tableName)
        {
            return new TimerOptions
            {
                Name = "request-timer",
                MeasurementUnit = Unit.Requests,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Context = $"{keyspace}.{tableName}"
            };
        }

        public static TimerOptions ClusterConnectTimer = new TimerOptions
        {
            Name = "cluster-connect-timer",
            MeasurementUnit = Unit.Connections,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public static TimerOptions ConnectionOpenTimer = new TimerOptions
        {
            Name = "open-connection-timer",
            MeasurementUnit = Unit.Connections,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public static TimerOptions WriteQueueTimer = new TimerOptions
        {
            Name = "write-queue-timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public static TimerOptions PendingRequestTimer = new TimerOptions
        {
            Name="pending-timer-options",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public static GaugeOptions ConnectedSessions = new GaugeOptions
        {
            Name = "connected-sessions",
            MeasurementUnit = Unit.Connections
        };

        public static GaugeOptions KnownHosts = new GaugeOptions
        {
            Name = "known-hosts",
            MeasurementUnit = Unit.Items
        };

        public static GaugeOptions ConnectedToHosts = new GaugeOptions
        {
            Name = "connected-to-hosts",
            MeasurementUnit = Unit.Items
        };

        public static GaugeOptions OpenConnections = new GaugeOptions
        {
            Name = "open-connections",
            MeasurementUnit = Unit.Connections
        };

        public static GaugeOptions InFlightRequests = new GaugeOptions
        {
            Name = "in-flight-requests",
            MeasurementUnit = Unit.Requests
        };
        
        
        public static GaugeOptions WriteQueueLength = new GaugeOptions
        {
            Name = "write-queue-length",
            MeasurementUnit = Unit.Requests
        };
        
        public static GaugeOptions FreeOperationsLength = new GaugeOptions
        {
            Name = "free-operations-length",
            MeasurementUnit = Unit.Items
        };

        public static CounterOptions NoHostAvailableErrors = new CounterOptions
        {
            Name = "no-host-available-error",
            MeasurementUnit = Unit.Errors
        };

        public static CounterOptions OperationTimeOutErrors = new CounterOptions
        {
            Name = "operation-time-out-errors",
            MeasurementUnit = Unit.Errors
        };
        
        public static CounterOptions WriteTimeOutErrors = new CounterOptions
        {
            Name = "write-time-out-errors",
            MeasurementUnit = Unit.Errors
        };
        
        public static CounterOptions ReadTimeOutErrors = new CounterOptions
        {
            Name = "read-time-out-errors",
            MeasurementUnit = Unit.Errors
        };

        public static CounterOptions UnavailableErrors = new CounterOptions
        {
            Name = "unavailable-errors",
            MeasurementUnit = Unit.Errors
        };

        public static CounterOptions OtherErrors = new CounterOptions
        {
            Name = "other-errors",
            MeasurementUnit = Unit.Errors
        };
    }
}