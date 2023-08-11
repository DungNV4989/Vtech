using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using VTECHERP.Debts;

namespace VTECHERP.BackgroundWorker
{
    public class BaseWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private const int DefaultTimer = 30 * 1000;
        private const int TimerForDay = 86400000;

        public BaseWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            SetTimer(DefaultTimer);
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            
        }
        public async Task LoadSettings()
        {
            SetTimer(TimerForDay);
        }
        private void SetTimer(int timer) =>
            Timer.Period = timer;
    }
}
