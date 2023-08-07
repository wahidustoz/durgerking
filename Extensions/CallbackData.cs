using System.Web;

namespace DurgerKing.Extensions;

public class CallbackData
{
    public CallbackData(string callbackQuery)
    {
        try
        {
            var values = HttpUtility.ParseQueryString(callbackQuery);
            int.TryParse(values["Index"], out var index);
            Name = values["Name"];
            Data = values["Data"];
            Index = index > 0 ? index : 1;
        }
        catch(Exception) { }
    }

    public string Name { get; set; }
    public string Data { get; set; }
    public int Index { get; set; } = 1;
}