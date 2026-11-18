using LibraryManagement.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Models;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;




namespace LibraryManagement.core.Services
{
    public class AuthService
    {
        private readonly LibraryDbContext context;
        private readonly IPasswordHasher<AppUser> passwordHasher;
        private readonly JwtService jwtService;

        public AuthService(LibraryDbContext context, IPasswordHasher<AppUser> passwordHasher, JwtService jwtService)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
            this.jwtService = jwtService;
        }

        public async Task RegisterAsync(string username,string password)
        {
            if(await context.Users.AnyAsync(u => u.Username==username))
            {
                throw new UserUnavailableException("This user before exist.");
            }

            var user = new AppUser
            {
                Username = username,
                Role = "User"
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var user=await context.Users.FirstOrDefaultAsync(u => u.Username== username);

            if(user==null)
            {
                return null;
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if(result==PasswordVerificationResult.Failed)
            {
                return null;
            }
                        
            return jwtService.GenerateToken(user);
        }


        public async Task<List<UserInfo>> GetUsersAsync()
        {
            return await context.Users.Select(u => new UserInfo
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
            }).ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(int userId,int requesterId, string role)
        {
            var user = await context.Users.FindAsync(userId);

            if(user==null )
            {
                return false;
            }

            if(role != "User" && role != "Admin")
            {
                throw new ArgumentException("Invalide role.");
            }

            if(userId==requesterId)
            {
                throw new Exception("You cannot change your own role.");
            }
            
            
            if (user.Role=="Admin" && role=="User")
            {
                var adminCount = await context.Users.CountAsync(u => u.Role == "Admin");
                    
                if(adminCount<= 1)
                {
                    throw new LastAdminCannotBeDemotedException();
                }

                
            }
            user.Role= role;
            await context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user= await context.Users.FindAsync(userId);

            if(user==null)
            {
                return false;
            }

            if(user.Role=="Admin")
            {
                var adminCount = await context.Users.CountAsync(u => u.Role == "Admin");

                if (adminCount <= 1)
                {
                    throw new LastAdminCannotBeDeletedException("Last admin cannot be deleted.");
                }
            }

            context.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
