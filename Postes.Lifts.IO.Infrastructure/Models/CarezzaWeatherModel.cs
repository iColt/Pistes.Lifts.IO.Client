namespace Postes.Lifts.IO.Infrastructure.Models;

public class CarezzaWeatherModel
{
    private const double NULL_VALUE = -1000.0;

    public double MaxTempToday { get; set; } = NULL_VALUE;

    public double MinTempToday { get; set; } = NULL_VALUE;

    public double MaxTempTomorrow { get; set; } = NULL_VALUE;

    public double MinTempTomorrow { get; set; } = NULL_VALUE;

    public bool Configured => MaxTempToday != NULL_VALUE 
        && MinTempToday != NULL_VALUE 
        && MaxTempTomorrow != NULL_VALUE 
        && MinTempTomorrow != NULL_VALUE;
}
