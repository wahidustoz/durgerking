using DurgerKing.Entity;

namespace DurgerKing.Dtos;

public class GetProductMediumDto
{
    public GetProductMediumDto(ProductMedia entity)
    {
        Filename = entity.Filename;
        Extension = entity.Extension;
        MimeType = entity.MimeType;
        Url = "";
        Order = entity.Order;
    }

    public string Filename { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
}