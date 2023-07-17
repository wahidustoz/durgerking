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
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            CategoryId = productdto.CategoryId
        });

        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = created.Entity.Id }, new GetProductDto(created.Entity));
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
        product.CategoryId = updateProduct.CategoryId;

        await dbContext.SaveChangesAsync();

        return Ok();
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

    [HttpPost("{id}/set")]
    public async Task<IActionResult> CreateSetDto(
        [FromRoute] Guid id,
        [FromBody] IEnumerable<Guid> itemIds,
        CreateSetDto setdto,
        CancellationToken cancellationToken = default)

    {
        var product = await dbContext.Products 
            .Where(a => a.Id == id && a.IsActive)
            .Include(a => a.Category)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();

        if(product.Category string.Equals(product.Category.Name, "set", ))
        {

        }
    

        if(itemIds.Contains(product.id))
        {
            return Conflict("This product already excist")
        }
        
        itemIds.Add(product.id);

        var products = await dbContext.Products.Items();
        foreach (var item in itemIds)
        {
            var temp = products.FirstOrDefaultAsync(p => p.Id == item);
            products.Items.Add(temp);
        }

        await dbContext.SaveChangesAsync();
        
    }

    // [HttpPost("{id}/set")]
    // public async Task<IActionResult> CreateSet (
    //     [FromRoute] Guid id, 
    //     [FromBody] IEnumerable<Guid> itemIds,
    //     CancellationToken cancellationToken = default)
    // {
    //     var product = await dbContext.Products
    //         .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    //     if(product is null || product is not IsActive)
    //     {
    //         return NotFound();
    //     }

    //     if(itemIds.Contains(product.id))
    //     {
    //         return Conflict("This product already excist")
    //     }
        
    //     itemIds.Add(product.id)


    //     return Ok();
    // }
}