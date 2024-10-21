namespace API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Authorize]
public class UsersController : BaseApiController
{

    private readonly IUserRepository _repository;

    public UsersController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet] // api/users
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]// api/users/2
    public async Task<ActionResult<AppUser>> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet("{username}")] // api/users/Patricio
    public async Task<ActionResult<AppUser>> GetByUsernameAsync(string username)
    {
        var user = await _repository.GetByUsernameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}