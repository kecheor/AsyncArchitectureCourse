﻿using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Contracts.Services;
/// <summary>
/// Producer adapter for event broker
/// Handles value serialization and publishing to the broker
/// </summary>
public interface IProducer : IDisposable
{
    Task<Either<None, Error>> Produce<TValue>(NewEventMessage<TValue> newEvent, CancellationToken cancellationToken) where TValue : IEventValue;
}
