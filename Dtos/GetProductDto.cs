namespace DurgerKing.Dtos;

public class GetProductDto
{
    public GetProductDto(Product entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Description = entity.Description;
        Price = entity.Price;
        DiscountPercentage = entity.DiscountPercentage;
        IsActive = entity.IsActive;
        CreatedAt = entity.CreatedAt;
        ModifiedAt = entity.ModifiedAt;
        CategoryId = entity.CategoryId;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public float DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int CategoryId { get; set; }
}