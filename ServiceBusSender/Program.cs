using Azure.Messaging.ServiceBus;


// connection string to your Service Bus namespace
string connectionString = "<NAMESPACE CONNECTION STRING>";

// name of your Service Bus queue
string queueName = "<QUEUE NAME>";

int nMessages = 5;

async void SendMessages(ServiceBusClient client, ServiceBusSender sender)
{
    for (int i = 0; i < nMessages; i++)
    {
        // create a message that we can send
        ServiceBusMessage message = new ServiceBusMessage($"Message {i} by SendMessages");
        // send the message
        await sender.SendMessageAsync(message);
        Console.WriteLine($"Sent message: {message.Body}");
    }
}


async void SendMessagesBatch(ServiceBusClient client, ServiceBusSender sender)
{
    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

    for (int i = 0; i < nMessages; i++)
    {
        // try adding a message to the batch
        if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i} by SendMessagesBatch")))
        {
            // if it is too large for the batch
            throw new Exception($"The message {i} is too large to fit in the batch.");
        }
    }

    // send the batch
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"Sent a batch of {nMessages} messages");
}


async void Main()
{
    ServiceBusClient client = new ServiceBusClient(connectionString);
    ServiceBusSender sender = client.CreateSender(queueName);

    try
    {
        Console.WriteLine("Sending messages to the Queue using SendMessages");
        SendMessages(client, sender);

        Console.WriteLine("Sending messages to the Queue using SendMessagesBatch");
        SendMessagesBatch(client, sender);
    }
    catch (Exception exception)
    {
        Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
    }
    finally
    {
        // Calling DisposeAsync on client types is required to ensure that network
        // resources and other unmanaged objects are properly cleaned up.
        await sender.DisposeAsync();
        await client.DisposeAsync();

        Console.WriteLine("Press any key to end the application");
        Console.ReadKey();
    }
}


Main();
