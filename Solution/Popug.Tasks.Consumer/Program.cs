using Popug.Tasks.Repository;

new AccountConsumer(new AccountRepository()).Run(new CancellationTokenSource().Token);
