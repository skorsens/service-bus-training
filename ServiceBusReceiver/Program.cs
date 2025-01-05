using Azure.Messaging.ServiceBus;


// connection string to your Service Bus namespace
string connectionString = "<NAMESPACE CONNECTION STRING>";

// name of your Service Bus queue
string queueName = "<QUEUE NAME>";


static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    // complete the message. messages is deleted from the queue.
    await args.CompleteMessageAsync(args.Message);
}


static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}


ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

try
{ 
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    Console.WriteLine("Starting the processor");

    // Start processing
    await processor.StartProcessingAsync();

    Console.WriteLine("Processing messages for a minute");
    Console.ReadKey();

    // Stop processing
    Console.WriteLine("Stopping the processor");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped the processor");
}
catch (Exception exception)
{
    Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
}
finally
{
    await processor.StopProcessingAsync();
    await processor.DisposeAsync();
    await client.DisposeAsync();

    Console.WriteLine("Press any key to end the application");
    Console.ReadKey();
}
