using Microsoft.Extensions.DependencyInjection;
using Postes.Lifts.IO.Infrastructure.Constants;
using Pistes.Lifts.IO.TGNotificator.Models;
using Microsoft.Extensions.Configuration;

namespace Pistes.Lifts.IO.TGNotificator;
class Program
{
    private static ServiceProvider ConfigureServices(IConfigurationRoot configModel)
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IBootstraper, Bootstraper>()
            //.AddTransient<IGeneralResponseHandler, GeneralResponseHandler>()
            .Configure<ConfigurationModel>(configModel.GetSection(Constants.AppSettings));

        return serviceProvider.BuildServiceProvider();
    }

    private static IConfigurationRoot BuildConfigurations()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Configs"))
            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    static void Main(string[] _)
    {
        var config = BuildConfigurations();

        var configModel = new ConfigurationModel();
        config.Bind(configModel);

        var serviceProvider = ConfigureServices(config);


        //Resolve bootstraper
        serviceProvider.GetService<IBootstraper>().Initialize();

        Console.ReadLine();
    }
}