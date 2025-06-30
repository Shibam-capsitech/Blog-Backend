using Microsoft.AspNetCore.Mvc;
using Practice_project1.Dto;
using Practice_project1.Models;
using Practice_project1.Service;
using System.Security.Claims;

namespace Practice_project1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public UserController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register (RegisterDto dto)
    {
        var user = await _userService.GetByUserNameAsync(dto.Username);
        if(user != null)
        {
            return BadRequest("User Already Exists" );
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var newUser = new UserModel
        {
            Name = dto.Name,
            Username = dto.Username,
            Email = dto.Email,
            Password = hashedPassword,
            City = dto.City,
            Age = dto.Age,
        };
        await _userService.CreateAsync(newUser);
        return Ok("User Created Successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.GetByUserNameAsync(dto.Username);
        if (user == null)
            return Unauthorized("No such user exists");
        if(!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return Unauthorized("Invalid cred");
        }

        var token = _jwtService.GenerateToken(user);
        return Ok(new { Token = token });
    }

    [HttpGet("getUserDetails/{id}")]
    public async Task<IActionResult> getUserDetails(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized("Unauthorized");
        }
        var user = await _userService.getUserDetails(id);
        if (user == null)
        {
            return NotFound("User Not Found");
        }
        return Ok(new { user });
    }

    [HttpGet("test-aggregation-count-newyork-over25")]
    public async Task<IActionResult> GetUserCountInNewYorkOver25()
    {
        var count = await _userService.CountUsersInNewYorkOver25Async();
        return Ok(new { count });
    }

    [HttpGet("test-aggregation-group-by-city")]
    public async Task<IActionResult> GroupCityAndUserCount()
    {
        var cities = await _userService.GroupUserBasedOnCity();
        return Ok(new { cities });
    }

    [HttpGet("test-aggregation-decend-by-age")]
    public async Task<IActionResult> GetUsersByAgeInDec()
    {
        var users = await _userService.GetUserByDecAge();
        return Ok(new { users });
    }

    [HttpGet("test-aggregation-get-active-user-from-newyork")]
    public async Task<IActionResult> GetActiveUserFromNewYork()
    {
        var users = await _userService.GetActiveUsersFromNewYork();
        return Ok(new { users });
    }
}
