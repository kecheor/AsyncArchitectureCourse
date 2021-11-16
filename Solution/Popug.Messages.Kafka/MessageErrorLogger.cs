using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Popug.Common;
using Popug.Common.Services;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Kafka
{
    public class MessageErrorLogger : IMessageErrorLogger
    {
        private const string TOPIC = "message-errors-stream";
        private readonly IProducer<Null, string> _producer;
        private readonly IJsonSerializer _serializer;
        private readonly ILogger _logger;

        public MessageErrorLogger(IProducer<Null, string> producer, IJsonSerializer serializer, ILogger logger)
        {
            _producer = producer;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task LogError(string source, string errorMessage, string? eventMessage, Exception? exception, CancellationToken cancellationToken)
        {
            var error = new MessageError(source, errorMessage, exception?.ToString() ?? string.Empty, eventMessage ?? string.Empty);
            var serialized = _serializer.Serialize(error);
            var message = new Message<Null, string>()
            {
                Value = serialized
            };
            DeliveryResult<Null, string> result;
            try
            {
                //For some unknown reason awaiting does not return
                var task = _producer.ProduceAsync(TOPIC, message, cancellationToken);
                _producer.Flush();
                await Task.Delay(1000);
                result = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not log message error '{errorMessage}' from '{source}'. Message: {eventMessage}");
                return;
            }

            if (result.Status != PersistenceStatus.Persisted)
            {
                _logger.LogError($"Could not log message error '{errorMessage}' from '{source}'. Message: {eventMessage}");
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
