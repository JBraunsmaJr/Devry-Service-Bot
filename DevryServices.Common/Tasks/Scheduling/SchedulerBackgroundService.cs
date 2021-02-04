﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServices.Common.Tasks.Scheduling
{
    public abstract class SchedulerBackgroundService : BackgroundService
    {
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnoservedTaskException;

        protected readonly IScheduledTaskService _taskService;

        public SchedulerBackgroundService(IServiceProvider serviceProvider)
        {
            var referenceTime = DateTime.Now;
            var tasks = serviceProvider.GetServices<IScheduledTask>();

            _taskService = serviceProvider.GetService<IScheduledTaskService>();

            // Task service is required. This will allow the addition / removal of tasks within this hosted service.
            // Otherwise there will be no way of adding/removing during runtime.
            if (_taskService == null)
                throw new InvalidOperationException($"{GetType().Name} - Requires a IScheduledTaskService to be registered");

            foreach(var task in tasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = referenceTime
                });
            }

            _taskService.OnAddTask += AddTask;
            _taskService.OnRemoveTask += RemoveTask;
        }

        public void RemoveTask(IScheduledTask task)
        {
            if(_scheduledTasks.Any(x=>x.Task.Id == task.Id))
            {
                var obj = _scheduledTasks.First(x => x.Task.Id == task.Id);
                _scheduledTasks.Remove(obj);
            }
        }

        public void AddTask(IScheduledTask task)
        {
            if(!_scheduledTasks.Any(x=>x.Task.Id == task.Id))
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = task.NextRunTime
                });
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        protected abstract Task ProcessTask(SchedulerTaskWrapper task, CancellationToken token);

        async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.Now;
            var tasksThatShouldRun = _scheduledTasks.Where(x => x.ShouldRun(referenceTime)).ToList();

            foreach(var task in tasksThatShouldRun)
            {
                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await ProcessTask(task, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            UnoservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                                throw;
                        }
                    }, cancellationToken);
            }
        }

    }
}
