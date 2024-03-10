using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using System;
using Postes.Lifts.IO.Infrastructure.Parsers;
using System.Text;


namespace Pistes.Lifts.IO.TGNotificator;
class Program
{
    //TODO
    private const string TOKEN = "TOKEN";

    private static IServiceProvider _serviceProvider;

    static readonly ITelegramBotClient bot = new TelegramBotClient(TOKEN);
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            //TODO: request per Context (where context ~ User)
            var message = update.Message;
            if (message.Text.Equals("/start", StringComparison.CurrentCultureIgnoreCase))
            {
                var model = CarezzaParser.Parse("URL");
                if(model.Configured)
                {
                    StringBuilder stringBuilder = new StringBuilder()
                        .AppendLine("Lift status:")
                        .AppendLine(model.LiftsStatus)
                        .AppendLine("Slopes status:")
                        .AppendLine(model.SlopesStatus);
                    await botClient.SendTextMessageAsync(update.Message.Chat, stringBuilder.ToString());
                }
                return;
            }
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }

    private static IServiceProvider ConfigureServices()
    {
        var serviceProvider = new ServiceCollection();
            //.AddTransient<IGeneralResponseHandler, GeneralResponseHandler>();

        return serviceProvider.BuildServiceProvider();
    }

    static void Main(string[] args)
    {
        _serviceProvider = ConfigureServices();

        Console.WriteLine("Bot started " + bot.GetMeAsync().Result.FirstName);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
    }
}