using Durgerking.Dtos;
using DurgerKing.Dtos;
using DurgerKing.Entity;
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
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productdto)
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
            .Include(u => u.Category)
            .Include(u => u.Items)
            .Select(u => new GetProductDto(u))
            .ToListAsync();

        var result = new PaginatedList<GetProductDto>(products, productsQuery.Count(), offset + 1, limit);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .Include(p => p.Category)
            .Include(p => p.Items)
            .FirstOrDefaultAsync();

        if(product is null)
            return NotFound();

        return Ok(new GetProductDto(product));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, UpdateProductDto updateProduct)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if(product is null)
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

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/set")]
    public async Task<IActionResult> CreateSet(
        [FromRoute] Guid id,
        [FromBody] IEnumerable<Guid> itemIds,
        CancellationToken cancellationToken = default)

    {
        var product = await dbContext.Products
            .Where(a => a.Id == id && a.IsActive)
            .Include(a => a.Category)
            .FirstOrDefaultAsync(cancellationToken);

        var setCategory = await dbContext.Categories
            .FirstAsync(p => p.Name == "Set", cancellationToken);

        if(product is null)
            return NotFound();

        if(product.CategoryId != setCategory.Id)
            return BadRequest("This product does not have category {set}");

        var items = await dbContext.Products
            .Where(p => itemIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if(items.Count < itemIds.Count())
            return BadRequest("Some items do not exist in system");

        if(items.Any(a => a.CategoryId == setCategory.Id))
            return BadRequest("Some items have category {set}. You cannot add them to set again");

        product.Items = items;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/media")]
    public async Task<IActionResult> CreateProductMedia(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == id && a.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();
        
        var files = Request.Form.Files;

        if(files?.Any() is not true)
            return NotFound();

        if (files.Any(f => f.Length > 5 * 1024 * 1024))
            return BadRequest("File size exceeds the limit.");

        var allowedFileExtensions = new[] { ".mp4", ".jpg", ".jpeg", ".png" };
        
        int order = 1;
        foreach(var file in files)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            if(!allowedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Allowed formats are: MP4, JPG, JPEG, PNG.");
            
            var created = dbContext.ProductMedias.Add(new ProductMedia
            {
                Id = Guid.NewGuid(),
                MimeType = file.ContentType,
                Filename = file.Name,
                Extension = fileExtension,
                Order = order++,
                Data = await GetFileData(file)
            });

            product.Media.Add(created.Entity);
        }

        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductMedia), new { id = product.Id }, product.Media);
    }

    [HttpGet("{id}/media")]
    public async Task<IActionResult> GetProductMedia(
        [FromRoute] Guid id,
        [FromQuery] string search,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == id && a.IsActive)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();

        var productMedias = product.Media
            .Select(u => new GetProductMediumDto(u)
            {
                Url = Url.Action(
                    action: nameof(GetProductMedium),
                    controller: nameof(ProductsController),
                    host: Request.Host.ToString(),
                    protocol: Request.Protocol,
                    values: new {MediaId = u.Id})
            });

        return Ok(productMedias);
    }

    [HttpGet("{productId}/media/{mediumId}")]
    public async Task<IActionResult> GetProductMedium(
        [FromRoute] Guid productId,
        [FromRoute] Guid mediumId,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == productId && a.IsActive)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();

        var media = product.Media
            .FirstOrDefault(a => a.Id == mediumId);

        if(media is null)
            return NotFound();

        return File(media.Data, media.MimeType);
    }

    [HttpDelete("{productId}/media/{mediumId}")]
    public async Task<IActionResult> DeleteMedium(
        [FromRoute] Guid productId,
        [FromRoute] Guid mediumId,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == productId && a.IsActive)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();

        var media = product.Media
            .FirstOrDefault(a => a.Id == mediumId);

        if(media is null)
            return NotFound();

        dbContext.ProductMedias.Remove(media);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    private async Task<byte[]> GetFileData(IFormFile formFile)
    {
        using(var memoryStream = new MemoryStream())
        {
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}