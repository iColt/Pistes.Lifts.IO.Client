using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Microsoft.Extensions.Options;
using Postes.Lifts.IO.Infrastructure.Models;
using Postes.Lifts.IO.Infrastructure.Handlers;

namespace Postes.Lifts.IO.Infrastructure.BotWrapper
{
    public interface IBotWrapper
    {
        void Initialize();
    }

    public class BotWrapper(IOptions<ConfigurationModel> configModel, IGeneralResponseHandler generalResponseHandler) : IBotWrapper
    {
        private readonly IOptions<IConfigurationModel> _configModel = configModel;
        private readonly IGeneralResponseHandler _generalResponseHandler = generalResponseHandler;


        private ITelegramBotClient _telegramBot;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            try
            {
                await _generalResponseHandler.HandleUpdateAsync(botClient, update, cancellationToken);
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

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public void Initialize()
        {
            _telegramBot = new TelegramBotClient(_configModel.Value.AuthToken);

            // Set webhook adress to bot

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
