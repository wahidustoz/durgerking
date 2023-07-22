using DurgerKing.Dtos;
using DurgerKing.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Controllers;


public partial class ProductsController
{
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
}