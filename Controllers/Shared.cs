using Durgerking.Dtos;
using DurgerKing.Dtos;
using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
public partial class ProductsController : ControllerBase
{
    private readonly IAppDbContext dbContext;

    public ProductsController(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
}