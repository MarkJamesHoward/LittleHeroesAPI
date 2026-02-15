using LittleHeroesAPI.Data;
using LittleHeroesAPI.DTOs;
using LittleHeroesAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    // POST api/auth/register
    // Used by QuickStart flow - creates a local guest account
    // In production, this would also create an Auth0 user via the Management API
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        // Create a local group and parent for the guest
        var group = new Group { Name = model.Email };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        var parent = new Parent
        {
            Auth0UserId = $"guest|{Guid.NewGuid()}",
            Email = model.Email,
            GroupId = group.Id
        };
        _db.Parents.Add(parent);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Registration successful", email = model.Email });
    }
}
