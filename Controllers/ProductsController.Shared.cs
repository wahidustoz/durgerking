using DurgerKing.Entity.Data;

namespace DurgerKing.Controllers;

public partial class ProductsController
{
    private readonly IAppDbContext dbContext;

    public ProductsController(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
}