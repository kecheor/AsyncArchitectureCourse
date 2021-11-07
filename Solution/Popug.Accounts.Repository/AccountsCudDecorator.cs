using Confluent.Kafka;
using Popug.Common.Events;
using System.Text.Json;

namespace Popug.Accounts.Repository
{
    public class AccountsCudDecorator : IAccountRepository, IDisposable
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProducer<string, string> _producer;
        private static string TOPIC = "popug-accounts";

        public AccountsCudDecorator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task<Account?> Add(Account account)
        {
            var result = await _accountRepository.Add(account);
            if(result == null)
                return result;

            ProduceAccountCreated(account);
            return result;
        }

        public Task<Account?> Find(int curvature)
        {
            return _accountRepository.Find(curvature);
        }

        public Task<IReadOnlyList<Account>> GetAll()
        {
            return _accountRepository.GetAll();
        }

        public Task<Account?> Update(Account account)
        {
            throw new NotImplementedException();
        }

        private void ProduceAccountCreated(Account account)
        {
            var value = new CudEvent(CudEventType.Created, JsonSerializer.Serialize(account));
             _producer.Produce(TOPIC, new Message<string, string> { Key = account.ChipId, Value = JsonSerializer.Serialize(value) });
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
