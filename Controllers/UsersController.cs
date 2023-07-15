using System.Linq;
using Durgerking.Dtos;
using DurgerKing.Dto;
using DurgerKing.Entity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAppDbContext dbContext;

    public UsersController(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string search, [FromQuery] int offset = 0, [FromQuery] int limit = 25)
    {
        var usersQuery = dbContext.Users.AsQueryable();
        if(false == string.IsNullOrWhiteSpace(search))
            usersQuery = usersQuery.Where(u => 
                u.Username.ToLower().Contains(search.ToLower()) ||
                u.Fullname.ToLower().Contains(search.ToLower()));
        
        var users = await usersQuery
            .Skip(limit * offset)
            .Take(limit)
            .Select(u => new GetUserDto(u))
            .ToListAsync();
        var result = new PaginatedList<GetUserDto>(users, usersQuery.Count(), offset + 1, limit);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] long id)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if(user is null)
            return NotFound();
            
        return Ok(new GetUserDto(user));
    }
}