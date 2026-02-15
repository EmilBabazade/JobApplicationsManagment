using MassTransit;
using Messages;
using System.Collections.Concurrent;

namespace NotificationService.MessageConsumers;

/// <summary>
/// Override ProcessMessage, don't use Consume it will handle duplicate and out of order messages ( Tho this can handle only 1 message concurrently )
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseMessageConsumer<T> : IConsumer<T> where T : BaseMessage
{
    // TODO: put to db or somewhere
    private static readonly ConcurrentDictionary<Guid, DateTimeOffset> _lastProcessedTimes = [];
    private static readonly ConcurrentDictionary<Guid, bool> _alreadyProcessed = [];
    public async Task Consume(ConsumeContext<T> context)
    {
        var message = context.Message;
        if (_alreadyProcessed.ContainsKey(message.Id))
        {
            Console.WriteLine($"Message with Id {message.Id} is already processed");
            return;
        }

        if (_lastProcessedTimes.TryGetValue(message.TimeStampId, out var lastProcessedDate))
        {
            if (lastProcessedDate > message.TimeStamp)
            {
                // TODO: log and handle out of order messages
                throw new NotImplementedException();
            }
        }

        await ProcessMessage(message);

        _alreadyProcessed[message.Id] = true;
        _lastProcessedTimes[message.TimeStampId] = message.TimeStamp;
    }

    protected virtual async Task ProcessMessage(T message)
    {
        throw new NotImplementedException();
    }
}