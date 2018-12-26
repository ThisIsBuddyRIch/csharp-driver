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
using System.Threading.Tasks;
using App.Metrics.Timer;

namespace Cassandra.Metrics
{
    public class TaskCompletionSourceWithMetrics<T>
    {
        private readonly MetricsManager _metricsManager;
        private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();
        private TimerContext? _timerContext;

        public Task<T> Task => _tcs.Task;

        public TaskCompletionSourceWithMetrics(MetricsManager metricsManager, MetricsTableMeta tableMeta)
        {
            _metricsManager = metricsManager;
            _timerContext = metricsManager.GetRequestTimerContext(tableMeta);
        }

        public bool TrySetResult(T result)
        {
            _timerContext?.Dispose();
            return _tcs.TrySetResult(result);
        }

        public bool TrySetException(Exception ex)
        {
            _metricsManager.ReportOnError(ex);
            return _tcs.TrySetException(ex);
        }
        
    }
}