using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Services;
using LibraryManagement.core.Services;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            await authService.RegisterAsync(dto.Username, dto.Password);

            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var token=await authService.LoginAsync(dto.Username, dto.Password);

            if(token==null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new {token});
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users=await authService.GetUsersAsync();

            return Ok(users);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDTO dto)
        {
            var requesterIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(requesterIdValue, out var requesterId))
                return Unauthorized();

            var result= await authService.UpdateUserRoleAsync(id,requesterId, dto.Role);
                        

            if(!result)
            {
                return NotFound("User not found.");
            }

            return Ok("User role updated successfully.");
        }


        [Authorize(Roles ="Admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result= await authService.DeleteUserAsync(id);

            if(!result)
            {
                return NotFound("User not found.");
            }

            return NoContent();
        }
    }
}
