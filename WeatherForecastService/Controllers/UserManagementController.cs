using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WeatherForecastService.Dtos.User;
using WeatherForecastService.Dtos.Config;

namespace WeatherForecastService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public UserManagementController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> jwtConfigOptions)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfigOptions.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    return BadRequest(new RequestValidationDto()
                    {
                        Errors = new List<string>() {
                                "The account already exists."
                            },
                        IsSuccess= false
                    });
                }

                var newUser = new User() { Email = user.Email, UserName = user.Username };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);
                if (isCreated.Succeeded)
                {
                    var jwtToken = GenerateToken(newUser);                  

                    return Ok(new RequestValidationDto()
                    {
                        IsSuccess = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest(new RequestValidationDto()
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        IsSuccess = false
                    });
                }
            }

            return BadRequest(new RequestValidationDto()
            {
                Errors = ModelState.Select(q => q.Key).ToList(),
                IsSuccess = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return BadRequest(new RequestValidationDto()
                    {
                        Errors = new List<string>() {
                                "A user with this email does not exist."
                            },
                        IsSuccess = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!isCorrect)
                {
                    return BadRequest(new RequestValidationDto()
                    {
                        Errors = new List<string>() {
                                "Password is incorrect."
                            },
                        IsSuccess = false
                    });
                }

                var jwtToken = GenerateToken(existingUser);

                return Ok(new RequestValidationDto()
                {
                    IsSuccess = true,
                    Token = jwtToken
                });
            }

            return BadRequest(new RequestValidationDto()
            {
                Errors = ModelState.Select(q => q.Key).ToList(),
                IsSuccess = false
            });
        }
        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var signingCredential = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_jwtConfig.ExpirationDays)),
                SigningCredentials = signingCredential,
                Issuer=_jwtConfig.Issuer,
                Audience=_jwtConfig.Audience
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}