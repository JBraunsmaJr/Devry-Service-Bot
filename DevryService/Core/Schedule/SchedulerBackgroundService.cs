﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryService.Core.Schedule
{
    public class SchedulerBackgroundService : BackgroundService
    {
        public static SchedulerBackgroundService Instance;
        private readonly ILogger<SchedulerBackgroundService> _logger;
        public SchedulerBackgroundService(ILogger<SchedulerBackgroundService> logger)
        {
            _logger = logger;
            Instance = this;
        }

        private Dictionary<string, SchedulerTaskWrapper> _scheduledTasks = new Dictionary<string, SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedException;
        
        public void RemoveTask(string id)
        {
            if (_scheduledTasks.ContainsKey(id))
            {
                _logger.LogInformation($"Scheduler removed task: {id}");
                _scheduledTasks.Remove(id);
            }
        }

        public void AddTask(IScheduledTask task)
        {
            if (!_scheduledTasks.ContainsKey(task.Id))
            {
                _scheduledTasks.Add(task.Id, new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = task.NextRunTime
                });

                _logger.LogInformation($"Scheduler added task: {task.Name} | {task.Schedule}");
            }
        }

        async Task initialize()
        {
            using (DevryDbContext context = new DevryDbContext())
            {
                foreach (var reminder in await context.Reminders.ToListAsync())
                {
                    AddTask(reminder);
                }
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken token)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);

            // Cached since this will be referenced quite frequently in this method
            var referenceTime = DateTime.Now;

            // Determine which tasks will never run again and remove them from the database
            var removeList = _scheduledTasks.Where(x => x.Value.WillNeverRunAgain).ToList();

            // We must ensure that our database is updated -- to remove tasks that will never run again
            using (DevryDbContext context = new DevryDbContext())
            {
                foreach (var pair in removeList)
                {
                    Models.Reminder reminder = await context.Reminders.FindAsync(pair.Key);
                    
                    if(reminder == null)
                    {
                        _logger.LogWarning($"Unable to locate reminder with Id: {pair.Key}");
                        continue;
                    }
                    else
                    {
                        _logger.LogInformation($"Reminder: '{reminder.Name}' with Id '{reminder.Id}' is being cleaned up. --Determined to never run again");
                        context.Reminders.Remove(reminder);
                        _scheduledTasks.Remove(pair.Key);
                    }
                }

                // Save changes (if applicable)
                await context.SaveChangesAsync();
            }

            var tasksThatShouldRun = _scheduledTasks.Values.Where(x => x.ShouldRun(referenceTime));

            foreach(var task in tasksThatShouldRun)
            {
                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.Task.ExecuteAsync(token);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            UnobservedException?.Invoke(this, args);

                            if (!args.Observed)
                                throw;
                        }
                    },
                    token);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await initialize();

            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
