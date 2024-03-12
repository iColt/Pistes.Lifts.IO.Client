using HtmlAgilityPack;
using Postes.Lifts.IO.Infrastructure.Models;

namespace Postes.Lifts.IO.Infrastructure.Parsers;

public class CarezzaParser
{
    private const string LIFTS_ELEMENT_NAME = "//div[@class='wysiwyg__notes']/text()";
    private const string WEATHER_ELEMENT_NAME = "//div[@class='weather-temperature__value']/text()";

    public static CarezzaResortModel ParseSlopesAndLifts(string url)
    {
        CarezzaResortModel model = new();

        // Load HTML content from the URL
        HtmlWeb web = new();
        HtmlDocument doc = web.Load(url);

        var nodes = doc.DocumentNode.SelectNodes(LIFTS_ELEMENT_NAME);

        if(nodes.Count() != 2 )
        {
            return model;
        }

        model.LiftsStatus = nodes[0].InnerText.Trim();
        model.SlopesStatus = nodes[1].InnerText.Trim();

        return model;
    }

    public static CarezzaWeatherModel ParseWeather(string url)
    {
        CarezzaWeatherModel model = new();

        // Load HTML content from the URL
        HtmlWeb web = new();
        HtmlDocument doc = web.Load(url);

        var nodes = doc.DocumentNode.SelectNodes(WEATHER_ELEMENT_NAME);

        if (nodes.Count() != 10)
        {
            return model;
        }


        model.MaxTempToday = GetTemp(nodes[0].InnerText);
        model.MinTempToday = GetTemp(nodes[1].InnerText);
        model.MaxTempTomorrow = GetTemp(nodes[2].InnerText);
        model.MinTempTomorrow = GetTemp(nodes[3].InnerText);

        return model;
    }

    private static double GetTemp(string input)
    {
        var str = input.Trim();
        double result = 0.0;
        double.TryParse(str.Substring(0, str.Length - 1), out result);
        return result;
    }
} 
