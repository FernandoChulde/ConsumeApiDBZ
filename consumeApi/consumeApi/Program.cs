using System;

namespace consumeApi
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Newtonsoft.Json;

    class Program
    {
        // Connection String for the namespace can be obtained from the Azure portal under the 
        // 'Shared Access policies' section.
        const string ServiceBusConnectionString = "Endpoint=sb://esbutn.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aufYE78xnWvQ7YN/f7AlMyqvFxM7NHfI9i/xtjr/kKc=";
        const string QueueName = "cola001";
        static IQueueClient queueClient;



        public static async Task Main(string[] args)
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages.
            await SendMessagesAsync();

            Console.ReadKey();

            await queueClient.CloseAsync();
        }



        static async Task SendMessagesAsync()
        {
            try
            {
                var endpoint = "https://dragon-ball-api.herokuapp.com/api/character/";
                var http = new HttpClient();
                var response = await http.GetStringAsync(endpoint);
                //var posts = JsonConvert.DeserializeObject<List<Character>>(response);
              
                var message = new Message(Encoding.UTF8.GetBytes(response));
                // Write the body of the message to the console

                //Console.WriteLine(response.GetType());
                //Console.WriteLine(response);

                //Send the message to the queue
                var personajes = JsonConvert.DeserializeObject<List<Character>>(Encoding.UTF8.GetString(message.Body));



                foreach (var personaje in personajes)
                {
                    Console.WriteLine("----------------------------");
                    Console.WriteLine($"Name:{personaje.name}");
                    Console.WriteLine($"Status:{ personaje.status}");
                    Console.WriteLine($"Species: { personaje.species}");
                    Console.WriteLine($"Series:{ personaje.series}");
                    Console.WriteLine($"Gender:{ personaje.gender}");
                    Console.WriteLine($"Origin Planet:{ personaje.originPlanet}");
                }
                await queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
