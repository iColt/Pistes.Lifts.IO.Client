using Postes.Lifts.IO.Infrastructure.Constants;
using Postes.Lifts.IO.Infrastructure.Parsers;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using Postes.Lifts.IO.Infrastructure.Models;

namespace Postes.Lifts.IO.Infrastructure.Handlers
{
    public interface IGeneralResponseHandler
    {
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }

    public class GeneralResponseHandler(IOptions<ConfigurationModel> configModel) : IGeneralResponseHandler
    {
        private readonly IOptions<ConfigurationModel> _configModel = configModel;

        //TODO: write to storage
        private static List<long> chatStorages = new();

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
    }
}
