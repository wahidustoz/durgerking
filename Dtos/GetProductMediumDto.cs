using DurgerKing.Entity;

namespace DurgerKing.Dtos;

public class GetProductMediumDto
{
    public GetProductMediumDto(ProductMedium entity)
    {
        Id = entity.Id;
        ProductId = entity.ProductId;
        Filename = entity.Filename;
        Extension = entity.Extension;
        MimeType = entity.MimeType;
        Order = entity.Order;
    }

    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Filename { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
}