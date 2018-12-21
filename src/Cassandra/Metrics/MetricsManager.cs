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
using System.IO;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Timer;

namespace Cassandra.Metrics
{
    public class MetricsManager
    {
        private readonly IMetricsRoot _root;
        private readonly ITimer _clusterConnectTimer;
        private readonly ITimer _connectionOpenTimer;
        private readonly ITimer _writeQueueTimer;
        private readonly ITimer _pendingRequestTimer;
        
        public MetricsManager(IMetricsRoot root)
        {
            _root = root;
            if (IsMetricsEnabled)
            {
                _clusterConnectTimer = _root.Provider.Timer.Instance(DriverMetricsRegistry.ClusterConnectTimer);
                _connectionOpenTimer = _root.Provider.Timer.Instance(DriverMetricsRegistry.ConnectionOpenTimer);
                _writeQueueTimer = _root.Provider.Timer.Instance(DriverMetricsRegistry.WriteQueueTimer);
                _pendingRequestTimer = _root.Provider.Timer.Instance(DriverMetricsRegistry.PendingRequestTimer);
            }
        }

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

        public void RegisterWriteQueueLengthGauge(Func<double> valueProvider)
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.WriteQueueLength, valueProvider);
        }

        public void RegisterFreeOperationsLengthGauge(Func<double> valueProvider)
        {
            if(!IsMetricsEnabled) return;
            _root.Measure.Gauge.SetValue(DriverMetricsRegistry.FreeOperationsLength, valueProvider);
        }


        public TimerContext? GetClusterConnectTimer()
        {
            if (!IsMetricsEnabled) return null;
            return _clusterConnectTimer.NewContext();
        }

        public TimerContext? GetRequestTimerContext(MetricsTableMeta tableMeta)
        {
            if (!IsMetricsEnabled) return null;
            return _root.Measure.Timer.Time(
                DriverMetricsRegistry
                    .GetRequestTimer(tableMeta?.Keyspace ?? "undefined-keyspace", tableMeta?.TableName ?? "undefined-table"));

        }

        public TimerContext? GetConnectionOpenTimer()
        {
            if (!IsMetricsEnabled) return null;
            return _connectionOpenTimer.NewContext();
        }

        public TimerContext? GetWriteQueueTimer()
        {
            if (!IsMetricsEnabled) return null;
            return _writeQueueTimer.NewContext();
        }

        public TimerContext? GetPendingRequestTimer()
        {
            if (!IsMetricsEnabled) return null;
            return _pendingRequestTimer.NewContext();
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
}