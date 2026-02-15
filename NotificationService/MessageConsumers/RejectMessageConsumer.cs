using Messages;
using NotificationService.Protos;

namespace NotificationService.MessageConsumers;

public class RejectMessageConsumer : BaseMessageConsumer<RejectionMessage>
{
    private readonly PersonSearch.PersonSearchClient _personSearchClient;
    public RejectMessageConsumer(PersonSearch.PersonSearchClient personSearchClient)
    {
        _personSearchClient = personSearchClient;
    }

    protected override async Task ProcessMessage(RejectionMessage message)
    {
        var person = await _personSearchClient.GetByIdAsync(new GetByIdRequest { Id = message.PersonId.ToString() });
        Console.WriteLine($"Received message {message}");
        Console.WriteLine($"Sending Unfortunately blah blah blah to {person.Email} after 2 weeks to waste their time");
        Console.WriteLine("---------------------------------------------------");
        // sending email...
        await Task.Delay(1000);
    }
}
