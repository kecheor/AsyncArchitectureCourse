using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Contracts.Services
{
    public class EventValueSerializer : IEventValueSerializer
    {
        private readonly IJsonSerializer _jsonSerializer;

        public EventValueSerializer(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public Either<string, Error> Serialize<TValue>(NewEventMessage<TValue> message) where TValue : IEventValue
        {
            return Serialize(message.EventName, message.Producer, message.Value);
        }

        public Either<string, Error> Serialize<TValue>(string eventName, string producer, TValue value) where TValue : IEventValue
        {
            var metadata = CreateMetadata(eventName, producer, value.Version);
            var serialized = _jsonSerializer.Serialize(value);
            var message = new SerializedEventMessage(metadata, serialized);
            try
            {
                return _jsonSerializer.Serialize(message);
            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }
        }

        public Either<EventMessage<TValue>, Error> Deserialize<TValue>(string json) where TValue : IEventValue
        {
            SerializedEventMessage message;
            try
            {
                message = _jsonSerializer.Deserialize<SerializedEventMessage>(json);
            }
            catch(Exception ex)
            {
                return new ExceptionError(ex);
            }
            
            try
            {
                var value = _jsonSerializer.Deserialize<TValue>(message.Value);
                if(value.Version != message.Metadata.DataVersion)
                {
                    return new MessageEventError($"Currently backward compatibilty is not supported. Target verion: {value.Version}. Message version: {message.Metadata.DataVersion}", json);
                }
                return new EventMessage<TValue>(message.Metadata, value);
            }
            catch(Exception ex)
            {
                try
                {
                    var version = _jsonSerializer.Deserialize<EventVersionFallback>(message.Value);
                    if (version.Version != message.Metadata.DataVersion)
                    {
                        return new MessageEventError($"Currently backward compatibilty is not supported. Target verion: {version.Version}. Message version: {message.Metadata.DataVersion}", json);
                    }
                    return new MessageEventError($"Could not deserialize message to {typeof(TValue).FullName}", json);
                }
                catch
                {
                    return new ExceptionError(ex);
                }
            }
        }

        private static EventMetadata CreateMetadata(string eventName, string producer, int valueVersion)
        {
            return new EventMetadata(Guid.NewGuid().ToString(), valueVersion, eventName, DateTime.UtcNow, producer);
        }

        private class EventVersionFallback : IEventValue
        {
            public int Version { init; get; }
        }
    }
}
