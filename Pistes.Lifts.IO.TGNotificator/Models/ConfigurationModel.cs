namespace Pistes.Lifts.IO.TGNotificator.Models;

public interface IConfigurationModel
{
    string AuthToken { get; set; }
    decimal ChatId { get; set; }
    string CarezzaSlopesLiftsPath { get; set; }
    string CarezzaWeatherPath { get; set; }
}

public class ConfigurationModel : IConfigurationModel
{
    public string AuthToken { get; set; } = string.Empty;
    public decimal ChatId { get; set; }
    public string CarezzaSlopesLiftsPath { get; set; } = string.Empty;
    public string CarezzaWeatherPath { get; set; } = string.Empty;
}
