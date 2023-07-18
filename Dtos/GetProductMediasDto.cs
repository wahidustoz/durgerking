using DurgerKing.Entity;

namespace DurgerKing.Dtos;

public class GetProductMediasDto
{
    public GetProductMediasDto(ProductMedia entity)
    {
        Filename = entity.Filename;
        Extension = entity.Extension;
        MimeType = entity.MimeType;
        Url = GetMediaUrl(entity);
        Order = entity.Order;
    }
    public string Filename { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }

    private string GetMediaUrl(ProductMedia media)
    {
        var baseUrl = "http://example.com/storage/";
        var url = baseUrl + media.Id.ToString("N") + media.Extension;
        return url;
    }
}