using MassTransit;
using Messages;
using NotificationService.Protos;
using System.Collections.Concurrent;

namespace NotificationService.MessageConsumers;

public class AppointmentSetMessageConsumer : IConsumer<AppointmentSetMessage>
{
    private readonly PersonSearch.PersonSearchClient _personSearchClient;
    private readonly ConcurrentDictionary<Guid, DateTimeOffset> _lastProcessedTimes = [];
    private readonly ConcurrentDictionary<Guid, bool> _alreadyProcessed = [];

    public AppointmentSetMessageConsumer(PersonSearch.PersonSearchClient personSearchClient)
    {
        _personSearchClient = personSearchClient;
    }
    public async Task Consume(ConsumeContext<AppointmentSetMessage> context)
    {
        var message = context.Message;
        if (_alreadyProcessed.ContainsKey(message.Id))
        {
            Console.WriteLine($"Message with Id {message.Id} is already processed");
            return;
        }

        if(_lastProcessedTimes.TryGetValue(message.ApplicationId, out var lastProcessedDate))
        {
            if (lastProcessedDate > message.TimeStamp)
            {
                // TODO: handle messages out of order
                throw new NotImplementedException();
            }
        }

        var person = await _personSearchClient.GetByIdAsync(new GetByIdRequest { Id = message.PersonId.ToString() });
        await SendEmail(message, person.Email);

        _alreadyProcessed[message.Id] = true;
        _lastProcessedTimes[message.ApplicationId] = message.TimeStamp;
    }

    private async Task SendEmail(AppointmentSetMessage message, string toEmail)
    {
        Console.WriteLine($"Received message {message}");
        Console.WriteLine($"Sending to {toEmail}");
        Console.WriteLine("---------------------------------------------------");
        // sending email...
        await Task.Delay(1000);
    }
}
