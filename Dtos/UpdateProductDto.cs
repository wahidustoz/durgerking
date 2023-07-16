namespace DurgerKing.Dtos;

public class UpdateProductDto
{   
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public float DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int CategoryId { get; set; }
}