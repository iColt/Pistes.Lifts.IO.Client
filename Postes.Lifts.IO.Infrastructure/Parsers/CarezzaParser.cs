using HtmlAgilityPack;
using Postes.Lifts.IO.Infrastructure.Models;

namespace Postes.Lifts.IO.Infrastructure.Parsers;

public class CarezzaParser
{
    private const string LIFTS_ELEMENT_NAME = "//div[@class='wysiwyg__notes']/text()";

    public static CarezzaResortModel Parse(string url)
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
}
