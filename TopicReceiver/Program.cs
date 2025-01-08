using Azure.Messaging.ServiceBus;


const string connectionString = "<NAMESPACE CONNECTION STRING>";
const string topicName = "skorsens-sb-training-topic";
const string subscriptionName = "skorsens-sb-training-topic-subscription-1";


ServiceBusClient client = new(connectionString);
ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

async Task MessageHandler(ProcessMessageEventArgs args)
{ 
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received message {body}");

    // Complete receiving the message and delete it from the subscription
    await args.CompleteMessageAsync(args.Message);
}


Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine($"Error receiving message: {args.Exception.ToString()}");
    return Task.CompletedTask;
}


try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    // Start processing
    await processor.StartProcessingAsync();

    Console.WriteLine("Receiving messages. Wait for a minute and then press any key to stop");
    Console.ReadKey();

    // Stop processing
    Console.WriteLine("Stopping the receiver");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopping the receiver done");
}
finally
{ 
    // Clean up any resources
    await processor.DisposeAsync();
    await client.DisposeAsync();
}
