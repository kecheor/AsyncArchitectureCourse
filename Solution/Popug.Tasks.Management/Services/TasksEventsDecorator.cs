using Popug.Common.Monads;
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
    private readonly IJsonSerializer _jsonSerializer;

    //TODO:From configuration
    private static string CUD_TOPIC = "popug-tasks-stream";
    private static string BE_TOPIC = "popug-tasks-events";

    public TasksEventsDecorator(ITasksService innerService, IProducer producer, IJsonSerializer serializer)
    {
        _inner = innerService;
        _producer = producer;
        _jsonSerializer = serializer;
    }

    public async Task<Either<None, Error>> Close(string performerId, string taskId, CancellationToken cancellationToken)
    {
        var result = await _inner.Close(performerId, taskId, cancellationToken);
        if (result.HasError)
        {
            return result.Error;
        }
        var value = _jsonSerializer.Serialize(new TaskStateChange(taskId, performerId, DateTime.UtcNow));
        //TODO Error handling
        return await _producer.Produce(BE_TOPIC, CreateMetadata(TaskBusinessEvent.Closed), value, cancellationToken);
    }

    public async Task<Either<TaskDto, Error>> Create(string manager, string taskDescription, CancellationToken cancellationToken)
    {
        var result = await _inner.Create(manager, taskDescription, cancellationToken);
        if (result.HasError)
        {
            return result.Error;
        }
        var newTask = result.Result;
        var cudValue = _jsonSerializer.Serialize(new SharedTask(newTask.Id, newTask.Description, newTask.PerformerId, newTask.State, newTask.Created));
        var beValue = _jsonSerializer.Serialize(new TaskStateChange(newTask.Id, newTask.PerformerId, newTask.Created));
        var cudEvent = _producer.Produce(CUD_TOPIC, CreateMetadata(CudEventType.Created), cudValue, cancellationToken);
        var beEvent = _producer.Produce(BE_TOPIC, CreateMetadata(TaskBusinessEvent.Assigned), beValue, cancellationToken);
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
            var beValue = _jsonSerializer.Serialize(new TaskStateChange(task.TaskId, task.PerformerId, task.Timestamp));
            var beEvent = _producer.Produce(BE_TOPIC, CreateMetadata(TaskBusinessEvent.Assigned), beValue, cancellationToken);
            events.Add(beEvent);
        }
        //TODO Error handling
        await Task.WhenAll(events);
        return Either<IReadOnlyList<StateChangeLog>, Error>.Success(result.Result);
    }

    private static EventMetadata CreateMetadata(string eventName)
    {
        return new EventMetadata(Guid.NewGuid().ToString(), 1, eventName, DateTime.UtcNow, nameof(TasksEventsDecorator));
    }
}
