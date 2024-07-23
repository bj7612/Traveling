using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Traveling.Dtos;
using Traveling.Models;
using Traveling.Services;

namespace Traveling.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;



        public AuthenticateController(
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }

        // 11-2 【理解】JWT与单点登录实例解释
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto  loginDto)
        {
            // Step1: using SignInManage to relize the Authetication through username and password (11-8 【应用】用户登陆)
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                false
            );

            if (!loginResult.Succeeded) return BadRequest();

            // when login successed, ge the user Identity
            var user = await _userManager.FindByNameAsync(loginDto.Email);


            // Step2: Create JWT
            // 2.1 header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;

            // 2.2 payload
            var claims = new List<Claim>();
            {
                new Claim(JwtRegisteredClaimNames.Sub, "fake_user_id");
                
                    // 11-5 【应用】添加用户角色
                //new Claim(ClaimTypes.Role, "Admin")
            };

            // add user role to claim
            var rolesNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in rolesNames) {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);       
            }

            // 2.3 signiture
            var secretKey = _configuration["Authentication:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("The secret key for authentication is not configured properly.");
            }

            var secretByte = Encoding.UTF8.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(secretByte);
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);


            // Step3: return 200 ok + jwt
            return Ok(tokenStr);
        }

        // 11-7 【应用】用户注册
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // 1 使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };

            // 2 hash密码，保存用户
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // 3 初始化购物车
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
            };
            await _touristRouteRepository.CreateShoppingCart(shoppingCart);
            await _touristRouteRepository.SaveAsync();

            // 4 return
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok();
        }
    }
}
