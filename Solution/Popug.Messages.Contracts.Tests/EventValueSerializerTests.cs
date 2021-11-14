using FluentAssertions;
using NUnit.Framework;
using Popug.Common.Services;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values;
using System;

namespace Popug.Messages.Contracts.Tests
{
    public class EventValueSerializerTests
    {
        EventValueSerializer serializer;

        [SetUp]
        public void Setup()
        {
            serializer = new EventValueSerializer(new CommonJsonSerializer());
        }

        [Test]
        public void EventValueSerializer_Serialize_ReturnSomething()
        {
            string TEST_VALUE = Guid.NewGuid().ToString();
            var value = new v1Value { Value = TEST_VALUE };
            var json = serializer.Serialize("unit.test", "nUnit", value);
            json.Apply(r => r.Should().NotBeNullOrEmpty(), e => e.Should().BeNull());
        }

        [Test]
        public void EventValueSerializer_Serialize_CanBeDeserialized()
        {
            string TEST_VALUE = Guid.NewGuid().ToString();
            var value = new v1Value { Value = TEST_VALUE };
            var json = serializer.Serialize("unit.test", "nUnit", value);
            json.Apply(r => serializer.Deserialize<v1Value>(r)
                .Apply(v =>
                {
                    v.Metadata.Version.Should().Be(value.Version);
                    v.Value.Version.Should().Be(value.Version);
                    v.Value.Value.Should().Be(value.Value);
                },
                e => e.Should().BeNull()),
                e => e.Should().BeNull());
        }

        [Test]
        public void EventValueSerializer_DeserializeNewVersion_Fails()
        {
            string TEST_VALUE = Guid.NewGuid().ToString();
            var value = new v1Value { Value = TEST_VALUE };
            var json = serializer.Serialize("unit.test", "nUnit", value).Result;
            serializer.Deserialize<v2Value>(json)
                .Apply(v => v.Should().BeNull(),
                e => e.Should().NotBeNull());
        }

        [Test]
        public void EventValueSerializer_DeserializeOtherType_Fails()
        {
            string TEST_VALUE = Guid.NewGuid().ToString();
            var value = new v1Value { Value = TEST_VALUE };
            var json = serializer.Serialize("unit.test", "nUnit", value).Result;
            serializer.Deserialize<OtherValue>(json)
                .Apply(v =>
                {
                    v.Value.Version.Should().Be(value.Version);
                },
                e => e.Should().NotBeNull());
        }


        private class v1Value : IEventValue
        {
            public string Value { get; set; }
            public int Version => 1;
        }

        private class v2Value : IEventValue
        {
            public string NewValue { get; set; }
            public int Version => 2;
        }

        private class OtherValue : IEventValue
        {
            public int Value { get; set; }
            public int Version => 1;
        }
    }
}