using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_management_backend.Data;
using user_management_backend.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;

namespace user_management_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(DataContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users
       [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(String id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
         public async Task<ActionResult<User>> PostUser(User user)
         {
             user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash); // Ensure `Password` matches the model's property name
             _context.Users.Add(user);
             await _context.SaveChangesAsync();

             return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
         }
       
        // POST: api/Users/login
        /* [HttpPost("login")]
         [AllowAnonymous]
         public async Task<IActionResult> Login([FromBody] LoginRequest request)
         {
             var user = await _context.Users.SingleOrDefaultAsync(u => u.username == request.username); // Ensure `Username` matches the model's property name
             if (user == null || _passwordHasher.VerifyHashedPassword(user, user.password, request.password) != PasswordVerificationResult.Success)
             {
                 return Unauthorized(new { message = "Invalid credentials" });
             }

             // Generate JWT token
             var token = await _tokenService.GenerateTokenAsync(user);

             return Ok(new
             {
                 accessToken = token
             });
         }*/

        // DELETE: api/Users/5
         [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteUser(String id)
         {
             var user = await _context.Users.FindAsync(id);
             if (user == null)
             {
                 return NotFound();
             }

             _context.Users.Remove(user);
             await _context.SaveChangesAsync();

             return NoContent();
         }

         private bool UserExists(String id)
         {
             return _context.Users.Any(e => e.Id == id);
         }
     }

        public class LoginRequest
        {
            public string username { get; set; } // Ensure casing consistency
            public string password { get; set; } // Ensure casing consistency
        }
    }

