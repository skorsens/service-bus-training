using System;
using System.Text;
using Microsoft.Azure.ServiceBus;


namespace TopicSender
{
    class Program
    {
        const string connectionString = "<NAMESPACE CONNECTION STRING>";
        const string topicName = "skorsens-sb-training-topic";
 
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            const int nMessages = 10;
            var topicClient = new TopicClient(connectionString, topicName);
            await SendMessagesAsync(nMessages, topicClient);
            Console.WriteLine($"Sending {nMessages} messages to topic {topicName}. Wait and press any key to finish");
            Console.ReadKey();
            await topicClient.CloseAsync();
        }

        static async Task SendMessagesAsync(int nMessages, TopicClient topicClient)
        {
            string messageBody = "";

            try
            {
                for (var i = 0; i < nMessages; i++)
                {
                    messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    Console.WriteLine($"Sending message {messageBody} to topic {topicName}");
                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Sending message {messageBody} to topic {topicName} failed. Exception {e.Message}");
            }
        }
    }
}
