namespace DurgerKing.Entity;

public class ProductMedium
{
    public Guid Id { get; set; }
    public string MimeType { get; set; }
    public string Filename { get; set; }
    public string Extension{ get; set; }
    public int Order { get; set; }
    public byte[] Data { get; set; }
    public Guid ProductId { get; set; }
    
    public virtual Product Product { get; set; }
}