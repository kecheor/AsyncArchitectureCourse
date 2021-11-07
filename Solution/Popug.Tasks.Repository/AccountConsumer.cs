using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Popug.Tasks.Repository
{
    public class AccountConsumer
    {
        private const string GROUP_ID = "test-consumer-group";
        private const string TOPIC = "popug-accounts";

        private readonly IAccountRepository _accountRepository;

        public AccountConsumer(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Run(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = GROUP_ID,
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<string, string>(conf).Build())
            {
                c.Subscribe(TOPIC);
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cancellationToken);
                            if(cr != null)
                            {
                                ConsumeAccountCUD(cr.Message.Key, cr.Message.Value);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }

        private System.Threading.Tasks.Task ConsumeAccountCUD(string key, string value)
        {
            Console.WriteLine(key);
            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}
