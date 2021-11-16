using Microsoft.EntityFrameworkCore;
using Popug.Common;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Tasks.Management.Models;
using Popug.Tasks.Repository;
using Popug.Tasks.Repository.Models;
using Popug.Tasks.Repository.Repositories;

namespace Popug.Tasks.Management.Services
{
    public class TasksService : ITasksService
    {
        private readonly IPerformerRepository _performerRepository;
        private readonly ITasksRepository _tasksRepository;
        public TasksService(ITasksRepository tasksRepository,IPerformerRepository performerRepository)
        {
            _tasksRepository = tasksRepository;
            _performerRepository = performerRepository;
        }

        public async Task<Either<IReadOnlyList<TaskDto>, Error>> Mine(string performer, CancellationToken cancellationToken)
        {
            var data = await _tasksRepository.FindByPerformer(performer, TaskState.Active, cancellationToken);

            return data
                .Select(t => new TaskDto(t.TaskPublicId, t.Text, t.Performer.Name, TaskState.Active, t.Created))
                .ToArray();
        }

        public async Task<Either<None, Error>> Close(string currentPopug, string taskId, CancellationToken cancellationToken)
        {
            var task = await _tasksRepository.FindByPublicId(taskId, cancellationToken);
            if (task == null)
            {
                return new Error("Task not found");
            }
            if (task.Performer.ChipId != currentPopug)
            {
                return new Error("Only performer can close the task");
            }
            if (task.Status != TaskState.Active)
            {
                return new Error("Only active tasks can be closed");
            }

            task.Status = TaskState.Closed;
            task.Updated = DateTime.UtcNow;
            //TODO: change to state change method
            await _tasksRepository.Save(task, cancellationToken);
            return Of.None();
        }

        public async Task<Either<TaskDto, Error>> Create(string currentPopug, string taskDescription, CancellationToken cancellationToken)
        {
            var performers = await _performerRepository.FindInRole(Roles.User, cancellationToken);
            if (performers == null || !performers.Any())
            {
                return new Error("Could not find sutable performer for the task");
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
            return new TaskDto(task.TaskPublicId, task.Text, task.Performer.Name, task.Status, task.Created);
        }

        public async Task<Either<IReadOnlyList<StateChangeLog>, Error>> Reassign(string currentPopug, CancellationToken cancellationToken)
        {
            var performers = await _performerRepository.FindInRole(Roles.User, cancellationToken);
            if (performers == null || !performers.Any())
            {
                return new Error("Could not find sutable performers for the tasks");
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
                changeLog.Add(new StateChangeLog(task.TaskPublicId, performer.ChipId, DateTime.UtcNow));
            }
            return changeLog;
        }
    }
}
