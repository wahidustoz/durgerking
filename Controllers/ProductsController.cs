using Durgerking.Dtos;
using DurgerKing.Dtos;
using DurgerKing.Entity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IAppDbContext dbContext;

    public ProductsController(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductdto productdto)
    {
        var created = dbContext.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = productdto.Name,
            Description = productdto.Description,
            Price = productdto.Price,
            DiscountPercentage = productdto.DiscountPercentage,
            IsActive = productdto.IsActive,
            CreatedAt = productdto.CreatedAt,
            ModifiedAt = productdto.ModifiedAt,
            CategoryId = productdto.CategoryId
        });

        await dbContext.SaveChangesAsync();

        return Ok(created.Entity.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string search, 
        [FromQuery] int offset = 0, 
        [FromQuery] int limit = 25)
    {
        var productsQuery = dbContext.Products.AsQueryable();
        
        if(false == string.IsNullOrWhiteSpace(search))
            productsQuery = productsQuery.Where(u => 
                u.Name.ToLower().Contains(search.ToLower()));

        var products = await productsQuery
            .Skip(limit * offset)
            .Take(limit)
            .Where(u => u.IsActive == true)
            .Select(u => new GetProductDto(u))
            .ToListAsync();

        var result = new PaginatedList<GetProductDto>(products, productsQuery.Count(), offset + 1, limit);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound();

        return Ok(new GetProductDto(product));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, UpdateProductDto updateProduct)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();

        product.Name = updateProduct.Name;
        product.Description = updateProduct.Description;
        product.Price = updateProduct.Price;
        product.DiscountPercentage = updateProduct.DiscountPercentage;
        product.IsActive = updateProduct.IsActive;
        product.CreatedAt = updateProduct.CreatedAt;
        product.ModifiedAt = updateProduct.ModifiedAt;
        product.CategoryId = updateProduct.CategoryId;

        await dbContext.SaveChangesAsync();

        return Ok(product.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(u => u.Id == id);

        if(product is null)
            return NotFound();
        
        product.IsActive = false;
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}