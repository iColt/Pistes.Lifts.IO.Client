using Postes.Lifts.IO.Infrastructure.Parsers;
using System.Text;
using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Microsoft.Extensions.Options;
using Postes.Lifts.IO.Infrastructure.Constants;
using Postes.Lifts.IO.Infrastructure.Models;

namespace Postes.Lifts.IO.Infrastructure.BotWrapper
{
    public interface IBotWrapper
    {
        void Initialize();
    }

    public class BotWrapper(IServiceProvider serviceProvider, IOptions<ConfigurationModel> configModel) : IBotWrapper
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IOptions<IConfigurationModel> _configModel = configModel;

        private static List<long> chatStorages = new();

        private ITelegramBotClient _telegramBot;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

                    if (message.Text.Equals(Constants.Constants.SkiReportCommand, StringComparison.CurrentCultureIgnoreCase))
                    {
                        await SendMessageWithCarezzaStatus(botClient, update);
                        return;
                    }

                    if (message.Text.Equals(Constants.Constants.WeatherReportCommand, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var model = CarezzaParser.ParseWeather(_configModel.Value.CarezzaWeatherPath);
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

        private async Task SendMessageWithCarezzaStatus(ITelegramBotClient botClient, Update update)
        {
            var model = CarezzaParser.ParseSlopesAndLifts(_configModel.Value.CarezzaSlopesLiftsPath);
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

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public void Initialize()
        {
            _telegramBot = new TelegramBotClient(_configModel.Value.AuthToken);

            Console.WriteLine("Bot started " + _telegramBot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };

            try
            {
                _telegramBot.StartReceiving(
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
                _telegramBot.CloseAsync();
            }
        }
    }
}
