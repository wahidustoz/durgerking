using Durgerking.Dtos;
using DurgerKing.Dtos;
using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Controllers;
public partial class ProductsController : ControllerBase
{
    [HttpPost("{id}/media")]
    public async Task<IActionResult> CreateProductMedia(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == id && a.IsActive)
            .Include(u => u.Media)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();
        
        var files = Request.Form.Files;

        if(files?.Any() is not true)
            return BadRequest("No files attached");
        
        if(files.Count() > 5 || files.Count() + product.Media.Count() > 5)
            return BadRequest("Total number of files must not exceed 5.");

        if (files.Any(f => f.Length > 5 * 1024 * 1024))
            return BadRequest("File size exceeds the limit.");

        var allowedFileExtensions = new[] { ".mp4", ".jpg", ".jpeg", ".png" };
        
        foreach(var file in files)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            if(!allowedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Allowed formats are: MP4, JPG, JPEG, PNG.");
            
            product.Media.Add(new ProductMedium
            {
                MimeType = file.ContentType,
                Filename = file.Name,
                Extension = fileExtension,
                Order = product.Media.Count() + 1,
                Data = await GetFileData(file)
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        var values = product.Media.Select(m => new GetProductMediumDto(m)
        {
            Url = GetMediumDownloadUrl(productId: product.Id, mediumId: m.Id)
        });

        return CreatedAtAction(
            actionName: nameof(GetProductMedia),
            routeValues: new { product.Id },
            value: values);
    }

    [HttpGet("{id}/media")]
    public async Task<IActionResult> GetProductMedia(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Where(a => a.Id == id && a.IsActive)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(cancellationToken);

        if(product is null)
            return NotFound();

        return Ok(product.Media.Select(u => new GetProductMediumDto(u)
        {
            Url = GetMediumDownloadUrl(productId: product.Id, mediumId: u.Id)
        }));
    }

    [HttpGet("{productId}/media/{mediumId}")]
    public async Task<IActionResult> DownloadProductMedium(
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

        dbContext.ProductMedia.Remove(media);
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
    
    private string GetMediumDownloadUrl(Guid productId, Guid mediumId)
        => Url.ActionLink(
            action: nameof(DownloadProductMedium),
            protocol: Request.Scheme,
            host: Request.Host.Value,
            values: new { productId, mediumId });
}
