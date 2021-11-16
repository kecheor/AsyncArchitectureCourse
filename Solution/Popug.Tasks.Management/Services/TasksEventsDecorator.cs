using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.BE.Tasks;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values.BE.Tasks.v1;
using Popug.Messages.Contracts.Values.CUD.Tasks.v1;
using Popug.Tasks.Management.Models;

namespace Popug.Tasks.Management.Services;
public class TasksEventsDecorator : ITasksService
{
    private readonly ITasksService _inner;
    //TODO:Lazy
    private readonly IProducer _producer;

    //TODO:From configuration
    private static string CUD_TOPIC = "popug-tasks-stream";
    private static string BE_TOPIC = "popug-tasks-events";

    public TasksEventsDecorator(ITasksService innerService, IProducer producer)
    {
        _inner = innerService;
        _producer = producer;
    }

    public async Task<Either<None, Error>> Close(string performerId, string taskId, CancellationToken cancellationToken)
    {
        var result = await _inner.Close(performerId, taskId, cancellationToken);
        if (result.HasError)
        {
            return result.Error;
        }
        var value = new TaskStateChange(taskId, performerId, DateTime.UtcNow);
        var message = new NewEventMessage<TaskStateChange>(BE_TOPIC, TaskBusinessEvent.Assigned, nameof(TasksEventsDecorator), value);
        //TODO Error handling
        return await _producer.Produce(message, cancellationToken);
    }

    public async Task<Either<TaskDto, Error>> Create(string manager, string taskDescription, CancellationToken cancellationToken)
    {
        var result = await _inner.Create(manager, taskDescription, cancellationToken);
        if (result.HasError)
        {
            return result.Error;
        }
        var newTask = result.Result;

        var cudValue = new TaskValue(newTask.Id, newTask.Description, newTask.PerformerId, newTask.State, newTask.Created);
        var cudMessage = new NewEventMessage<TaskValue>(CUD_TOPIC, CudEventType.Created, nameof(TasksEventsDecorator), cudValue);
        var cudEvent = _producer.Produce(cudMessage, cancellationToken);

        var beValue = new TaskStateChange(newTask.Id, newTask.PerformerId, newTask.Created);
        var beMessage = new NewEventMessage<TaskStateChange>(BE_TOPIC, TaskBusinessEvent.Assigned, nameof(TasksEventsDecorator), beValue);
        var beEvent = _producer.Produce(beMessage, cancellationToken);
        //TODO Error handling
        await Task.WhenAll(cudEvent, beEvent);
        return newTask;
    }

    public async Task<Either<IReadOnlyList<TaskDto>, Error>> Mine(string performer, CancellationToken cancellationToken)
    {
        return await _inner.Mine(performer, cancellationToken);
    }

    public async Task<Either<IReadOnlyList<StateChangeLog>, Error>> 
        Reassign(string manager, CancellationToken cancellationToken)
    {
        var result = await _inner.Reassign(manager, cancellationToken);
        if (result.HasError)
        {
            return result.Error;
        }
        var events = new List<Task<Either<None, Error>>>();
        foreach(var task in result.Result)
        {
            var beValue = new TaskStateChange(task.TaskId, task.PerformerId, task.Timestamp);
            var beMessage = new NewEventMessage<TaskStateChange>(BE_TOPIC, TaskBusinessEvent.Assigned, nameof(TasksEventsDecorator), beValue);
            var beEvent = _producer.Produce(beMessage, cancellationToken);
            events.Add(beEvent);
        }
        //TODO Error handling
        await Task.WhenAll(events);
        return Either<IReadOnlyList<StateChangeLog>, Error>.Success(result.Result);
    }
}
