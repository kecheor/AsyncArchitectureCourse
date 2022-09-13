using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Popug.Billing.Pricing.Models;
using Popug.Billing.Pricing.Services;
using System;

namespace Popug.Billing.Pricing.Tests;

[TestFixture]
public class BillingMessageGeneratorTests
{
    private readonly Fixture _fixture = new Fixture();

    [Test]
    public void GenerateAssign_AllPlaceholders_Replaced()
    {
        var config = CreateConfiguration();
        var service = new TransactionMessageGenerator(config);
        var transaction = CreateRandomTransaction();
        var task = _fixture.Create<string>();
        var result = service.CreateTaskAssignWithdrawalMessage(transaction, task);

        result.Should().BeEquivalentTo(transaction, config => config.Excluding(s => s.Description));
        result.Description.Should().Be($"Withdrawing ${transaction.Withdrawal} for assigment of '{task}'");
    }

    [Test]
    public void GenerateReward_AllPlaceholders_Replaced()
    {
        var config = CreateConfiguration();
        var service = new TransactionMessageGenerator(config);
        var transaction = CreateRandomTransaction();
        var task = _fixture.Create<string>();
        var result = service.CreateTaskCompletePayoutMessage(transaction, task);

        result.Should().BeEquivalentTo(transaction, config => config.Excluding(s => s.Description));
        result.Description.Should().Be($"Rewarding with ${transaction.Deposit} for completing of '{task}'");
    }

    private static TestConfiguration CreateConfiguration()
    {
        return new TestConfiguration()
        {
            BillingAmmoutPlaceholder = "{ammount}",
            TaskDescriptionPlaceholder = "{taskDesription}",
            TaskAssignWithdrawalTemplate = "Withdrawing ${ammount} for assigment of '{taskDesription}'",
            TaskCompletePayoutTemplate = "Rewarding with ${ammount} for completing of '{taskDesription}'"
        };
    }

    private BillingTransaction CreateRandomTransaction()
    {
        var rand = new Random();
        var transaction = new BillingTransaction(_fixture.Create<string>(), rand.Next(100), rand.Next(100), _fixture.Create<string>());
        return transaction;
    }

    private class TestConfiguration : ITransactionMessageConfiguration
    {
        public string BillingAmmoutPlaceholder { set; get; }

        public string TaskDescriptionPlaceholder { set; get; }

        public string TaskAssignWithdrawalTemplate { set; get; }

        public string TaskCompletePayoutTemplate { set; get; }
    }
}
