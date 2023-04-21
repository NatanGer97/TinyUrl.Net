using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Services;

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        public UsersController(UserService userService, IConfiguration configuration, IMapper mapper)
        {
            this.configuration = configuration; 
            this.userService = userService;
            this.mapper = mapper;
        }
        

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsersAync()
        {
            List<User> users = await userService.GetUsersAync();
            return Ok(mapper.Map<List<UserDtoOut>>(users));

        }

        [HttpPost, Route("Register")]
        public async Task<ActionResult<User>> RegisterUserAsync([FromBody] UserRegisterReqDto registerReqDto)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("error");
                return BadRequest();
            }

            User? userFromDb = await userService.FindUserByUserNameAsync(registerReqDto.Email);
            if (userFromDb != null)
            {
                return BadRequest("Email already exists");
            }

            // create new user
            User user = await userService.CreateUser(registerReqDto);

            return Ok(user);
        }


        [HttpPost, Route("Login")]
        public async Task<ActionResult<User>> LoginUserAsync([FromBody] UserLoginReqDto loginReqDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User? userFromDb = await userService.FindUserByUserNameAsync(loginReqDto.Email);
            if (userFromDb == null)
            {
                return Unauthorized("Email or password is incorrect");
            }

            // check password
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string hashedPassFromDb = userFromDb.Password;
            string passFromReq = loginReqDto.Password;
            PasswordVerificationResult passwordVerificationResult = passwordHasher.VerifyHashedPassword(string.Empty, hashedPassFromDb, passFromReq);
            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                // pass correct -> create  token
                string jwtToken = GenerateToken(userFromDb);

                return Ok(jwtToken);
            }
            else
            {
                return Unauthorized("Email or password is incorrect");

            }

        }

        private string GenerateToken(User user)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("JwtConfig:Secret").Value);

            // token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims: new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(1).ToString()),


                }),
                Expires = DateTime.UtcNow.AddSeconds(10),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtSecurityTokenHandler.WriteToken(token);

            return jwtToken;
        
        }
    }
}
