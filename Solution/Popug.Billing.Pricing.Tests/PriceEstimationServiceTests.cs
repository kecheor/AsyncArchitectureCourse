using FluentAssertions;
using NUnit.Framework;
using Popug.Billing.Pricing.Models;
using Popug.Billing.Pricing.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Popug.Billing.Pricing.Tests;

[TestFixture]
public class PriceEstimationServiceTests
{
    [Test]
    [Repeat(1000)]
    public async Task Generated_Random_WithinRange()
    {
        var config = CreateRandomConfiguration();

        await AssertValid(config);
    }

    [Test]
    public async Task Generated_CurrentRequirements_WithinRange()
    {
        var config = new TestConfiguration
        {
            MinPrice = 10m,
            MaxPrice = 20m,
            MinReward = 20m,
            MaxReward = 40m
        };

        await AssertValid(config);
    }

    [Test]
    public async Task Generated_AllEqual_WithinRange()
    {
        var config = new TestConfiguration
        {
            MinPrice = 10m,
            MaxPrice = 10m,
            MinReward = 10m,
            MaxReward = 10m
        };

        await AssertValid(config);
    }

    private static async Task AssertValid(TestConfiguration config)
    {
        var service = new PriceEstimationService(config);
        var prices = await service.EstimateTaskPrices("", default(CancellationToken));

        prices.HasError.Should().BeFalse();
        prices.Result.AssignPrice.Should().BeInRange(config.MinPrice, config.MaxPrice, "Assign price should be withing defined range");
        prices.Result.CompleteReward.Should().BeInRange(config.MinReward, config.MaxReward, "Assign price should be withing defined range");
    }

    private static TestConfiguration CreateRandomConfiguration()
    {
        var rand = new Random();
        var minPrice = rand.NextDouble() * 50;
        var maxPrice = minPrice + rand.NextDouble() * 50;
        var minReward = rand.NextDouble() * 50;
        var maxReward = minReward + rand.NextDouble() * 50;
        return new TestConfiguration
        {
            MinPrice = (decimal)minPrice,
            MaxPrice = (decimal)maxPrice,
            MinReward = (decimal)minReward,
            MaxReward = (decimal)maxReward
        };
    }

    private class TestConfiguration : IPriceEstimationConfiguration
    {
        public decimal MinPrice { set; get; }

        public decimal MaxPrice { set; get; }

        public decimal MinReward { set; get; }

        public decimal MaxReward { set; get; }
    }
}
