// 
//       Copyright (C) 2018 DataStax Inc.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 

using System;
using System.Collections.Generic;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Timer;

namespace Cassandra.Metrics
{
    public class MetricsManager
    {
        private readonly IMetricsRoot _root;


        public MetricsManager(IMetricsRoot root)
        {
            _root = root;
        }

        public IMetricsRoot MetricsRoot => _root;

        public bool IsMetricsEnabled => _root != null;

        public void RegisterKnownHostsGauge(Func<double> valueProvider)
        {
            if (!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.KnownHosts, valueProvider);
        }

        public void RegisterConnectedSessionGauge(Func<double> valueProvider)
        {
            if (!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.ConnectedSessions, valueProvider);
        }

        public void RegisterConnectedToHostsGauge(Func<double> valueProvider)
        {
            if (!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.ConnectedToHosts, valueProvider);
        }

        public void RegisterOpenConnectionGauge(Func<double> valueProvider)
        {
            if (!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.OpenConnections, valueProvider);
        }

        public void RegisterInFlightRequestGauge(Func<double> valueProvider)
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.InFlightRequests, valueProvider);
        }

        public TimerContext? GetClusterConnectTimer()
        {
            if (!IsMetricsEnabled) return null;
            return _root.Measure.Timer.Time(DriverMetricsRegistry.ClusterConnectTimer);
        }

        public void IncrementNoHostAvailableErrorCounter()
        {
            if (!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.NoHostAvailableErrors);
        }

        public void IncrementOperationTimeOutErrorCounter()
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.OperationTimeOutErrors);
        }

        public void IncrementWriteTimeOutErrorCounter()
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.WriteTimeOutErrors);
        }
        
        public void IncrementReadTimeOutErrorCounter()
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.ReadTimeOutErrors);
        }

        public void IncrementUnavailableErrorCounter()
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.UnavailableErrors);
        }

        public void IncrementOtherErrorsCounter()
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Counter.Increment(DriverMetricsRegistry.OtherErrors);
        }

        public void ReportOnError(Exception ex)
        {
            if(!IsMetricsEnabled) return;
            switch (ex)
            {
                case WriteTimeoutException _ : IncrementWriteTimeOutErrorCounter(); break; 
                case ReadTimeoutException _ :IncrementReadTimeOutErrorCounter(); break;
                case OperationTimedOutException _ : IncrementOperationTimeOutErrorCounter(); break;
                case NoHostAvailableException _ : IncrementNoHostAvailableErrorCounter(); break;
                case UnavailableException _ : IncrementUnavailableErrorCounter(); break;
                default: IncrementOtherErrorsCounter(); break;
            }
        }

    }

    public static class DriverMetricsRegistry
    {
        public static TimerOptions GetRequestTimer(string keyspace, string tableName)
        {
            return new TimerOptions
            {
                Name = "request-timer",
                MeasurementUnit = Unit.Items,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Context = $"{keyspace}.{tableName}"
            };
        }

        public static TimerOptions ClusterConnectTimer = new TimerOptions
        {
            Name = "cluster-connect-timer",
            MeasurementUnit = Unit.Items,
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


//    public static GaugeOptions ConnectedHostsPerSession(string sessionKeySpace)
//    {
//        return new GaugeOptions
//        {
//            Name = "connected-hosts-per-session",
//            MeasurementUnit = Unit.Items,
//            Context = $"{sessionKeySpace ?? undefinedSessionKeyspace}"
//        };
//    }

//    public static GaugeOptions ConnectionPerHostAndSession(string sessionKeySpace, string ipAddress)
//    {
//        return new GaugeOptions
//        {
//            Name = "connection-per-hosts-and-sessions",
//            MeasurementUnit = Unit.Connections,
//            Context = $"{sessionKeySpace ?? undefinedSessionKeyspace}.{ipAddress}"
//        };
//    }
    }
}