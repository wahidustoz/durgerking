namespace DurgerKing.Extensions;

public class InlinePaginationOptions
{
    public string Name { get; set; } = "pagination";
    public string NextButtonText { get; set; } = "➡️";
    public string PreviousButtonText { get; set; } = "⬅️";
    public string CurrentButtonText { get; set; } = "⭐";
    public int MaxButtonsPerRow { get; set; } = 5;
}
