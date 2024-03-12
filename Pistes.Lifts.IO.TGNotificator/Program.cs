using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Postes.Lifts.IO.Infrastructure.Parsers;
using System.Text;
using Postes.Lifts.IO.Infrastructure.Constants;
using Telegram.Bot.Exceptions;

namespace Pistes.Lifts.IO.TGNotificator;
class Program
{
    //TODO
    private const string TOKEN = "TOKEN";

    private static IServiceProvider _serviceProvider;
    private static List<long> chatStorages = new();


    static readonly ITelegramBotClient bot = new TelegramBotClient(TOKEN);

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

        try
        {
            //UpdateType.MyChatMember - when bot left
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                if (message.Type == Telegram.Bot.Types.Enums.MessageType.ChatMembersAdded)
                {
                    chatStorages.Add(update.Message.Chat.Id);

                    StringBuilder stringBuilder = new StringBuilder()
                            .AppendLine("/skiReport - Check slopes and lifts status")
                            .AppendLine("/weather - Check weather conditions");
                    await botClient.SendTextMessageAsync(update.Message.Chat, stringBuilder.ToString());
                    return;
                }

                if (message.Type == Telegram.Bot.Types.Enums.MessageType.ChatMemberLeft)
                {
                    Console.WriteLine(LogMessages.InfoMessageBotLeftTheChat);
                    return;
                }

                if (message == null || string.IsNullOrEmpty(message.Text))
                {
                    Console.WriteLine(LogMessages.WarningMessage1);
                    return;
                }

                if (message.Text.Equals(Constants.SkiReportCommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    await SendMessageWithCarezzaStatus(botClient, update);
                    return;
                }

                if (message.Text.Equals(Constants.WeatherReportCommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    var model = CarezzaParser.ParseWeather("2");
                    if (model.Configured)
                    {
                        StringBuilder stringBuilder = new StringBuilder()
                            .AppendLine($"Temperature today: Max: {model.MaxTempToday} | Min : {model.MinTempToday}")
                            .AppendLine($"Temperature tomorrow: Max: {model.MaxTempTomorrow} | Min : {model.MinTempTomorrow}");
                        await botClient.SendTextMessageAsync(update.Message.Chat, stringBuilder.ToString());
                    }
                    return;
                }
            }
        }
        catch (ApiRequestException are)
        {
            Console.WriteLine(are.ToString());
            //Hope that this exception will not affect bot
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;

        }
    }

    private static async Task SendMessageWithCarezzaStatus(ITelegramBotClient botClient, Update update)
    {
        var model = CarezzaParser.ParseSlopesAndLifts("1");
        if (model.Configured)
        {
            StringBuilder stringBuilder = new StringBuilder()
                .AppendLine("Lift status:")
                .AppendLine(model.LiftsStatus)
                .AppendLine("Slopes status:")
                .AppendLine(model.SlopesStatus);
            await botClient.SendTextMessageAsync(update.Message.Chat, stringBuilder.ToString());
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

    static void Main(string[] _)
    {
        _serviceProvider = ConfigureServices();

        Console.WriteLine("Bot started " + bot.GetMeAsync().Result.FirstName);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };

        try
        {
            bot.StartReceiving(
               HandleUpdateAsync,
               HandleErrorAsync,
               receiverOptions,
               cancellationToken
            );
            Console.ReadLine();
        }
        finally
        {
            cts.Cancel();
            bot.CloseAsync();
        }
    }
}