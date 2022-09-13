using Microsoft.EntityFrameworkCore;
using Popug.Common.Enums;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Tasks.Management.Models;
using Popug.Tasks.Repository.Models;
using Popug.Tasks.Repository.Repositories;

namespace Popug.Tasks.Management.Services
{
    public class TasksService : ITasksService
    {
        private readonly IPerformerRepository _performerRepository;
        private readonly ITasksRepository _tasksRepository;
        private readonly ILogger<ITasksService> _logger;
        public TasksService(ITasksRepository tasksRepository, IPerformerRepository performerRepository, ILogger<ITasksService> logger)
        {
            _tasksRepository = tasksRepository;
            _performerRepository = performerRepository;
            _logger = logger;
        }

        public async Task<Either<IReadOnlyList<TaskDto>, Error>> Mine(string performer, CancellationToken cancellationToken)
        {
            var data = await _tasksRepository.FindByPerformer(performer, TaskState.Active, cancellationToken);
            _logger.LogTrace($"Found {data.Count} tasks for {performer}");
            return data
                .Select(t => new TaskDto(t.TaskPublicId, t.Text, t.Performer.Name, TaskState.Active, t.Created))
                .ToArray();
        }

        public async Task<Either<None, Error>> Close(string currentPopug, string taskId, CancellationToken cancellationToken)
        {
            var task = await _tasksRepository.FindByPublicId(taskId, cancellationToken);
            if (task == null)
            {
                var message = $"Can't close task {taskId}. Task not found.";
                _logger.LogWarning(message);
                return new Error(message);
            }
            if (task.Performer.ChipId != currentPopug)
            {
                var message = $"Can't close task {taskId}. Only performer can close the task.";
                _logger.LogWarning(message);
                return new Error(message);
            }
            if (task.Status != TaskState.Active)
            {
                var message = $"Can't close task {taskId}. Only active tasks can be closed.";
                _logger.LogWarning(message);
                return new Error(message);
            }

            task.Status = TaskState.Closed;
            task.Updated = DateTime.UtcNow;
            //TODO: change to state change method
            await _tasksRepository.Save(task, cancellationToken);
            _logger.LogInformation($"Closed task {taskId}");
            return Of.None();
        }

        public async Task<Either<TaskDto, Error>> Create(string currentPopug, string taskDescription, CancellationToken cancellationToken)
        {
            var performers = await _performerRepository.FindInRole(Roles.User, cancellationToken);
            if (performers == null || !performers.Any())
            {
                var message = $"Can't create new task. Could not find sutable performer for the task.";
                _logger.LogWarning(message);
                return new Error(message);
            }
            var lucky = new Random().Next(performers.Count);
            var performer = performers[lucky];
            var now = DateTime.UtcNow;
            var task = new TaskData()
            {
                TaskPublicId = Guid.NewGuid().ToString(),
                Text = taskDescription,
                Performer = performer,
                Status = TaskState.Active,
                Created = now,
                Updated = now,
            };
            var log = new TaskPerformerLog() { TaskId = task.Id.Value, PerformerId = performer.Id.Value, Assigned = now };
            await Task.WhenAll(
                _tasksRepository.Add(task, cancellationToken),
                _performerRepository.LogAssign(log, cancellationToken));
            _logger.LogInformation($"Created new task {task.Id} and assigned to {performer.Id}");
            return new TaskDto(task.TaskPublicId, task.Text, task.Performer.Name, task.Status, task.Created);
        }

        public async Task<Either<IReadOnlyList<StateChangeLog>, Error>> Reassign(string currentPopug, CancellationToken cancellationToken)
        {
            var performers = await _performerRepository.FindInRole(Roles.User, cancellationToken);
            if (performers == null || !performers.Any())
            {
                var message = $"Can't reassign task. Could not find sutable performers for the tasks.";
                _logger.LogWarning(message);
                return new Error(message);
            }

            var tasks = await _tasksRepository.FindByState(TaskState.Active, cancellationToken);
            var changeLog = new List<StateChangeLog>(tasks.Count);
            foreach (var task in tasks)
            {
                var lucky = new Random().Next(performers.Count);
                var performer = performers[lucky];

                task.Performer = performer;
                var log = new TaskPerformerLog() { TaskId = task.Id.Value, PerformerId = performer.Id.Value, Assigned = DateTime.UtcNow };
                //WARNING This also commits task change
                //TODO: remove awaits from the loop
                await _performerRepository.LogAssign(log, cancellationToken);
                _logger.LogInformation($"Assigned task {task.Id} and assigned to {performer.Id}");
                changeLog.Add(new StateChangeLog(task.TaskPublicId, performer.ChipId, DateTime.UtcNow));
            }
            _logger.LogInformation($"Reassigned {changeLog.Count} tasks");
            return changeLog;
        }
    }
}
