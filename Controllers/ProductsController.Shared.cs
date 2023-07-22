using DurgerKing.Entity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DurgerKing.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    protected readonly IAppDbContext dbContext;

    public ProductsController(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
}