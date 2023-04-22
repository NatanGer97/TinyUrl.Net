using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TinyUrl.Errors;
using TinyUrl.Models;
using TinyUrl.Services;
using TinyUrl.Services.interfaces;

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserClicksController : ControllerBase
    {
        private readonly IUserClickService userClickService;
        private readonly UserService userService;
        public UserClicksController(IUserClickService userClickService, UserService userService)
        {
            this.userClickService = userClickService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserClicksAsync([FromQuery, Required] string username, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
        {

            if (await userService.FindUserByUserNameAsync(username) == null)
            {
                throw new NotFoundException($"User with username: {username} not found");
            }
            
            PagedData<UserClick> pagedData = await userClickService.UserClicks(username, pageNum, pageSize);
            return Ok(pagedData);
        }

        [HttpGet("by_tiny")]
        public async Task<IActionResult> GetByTiny([FromQuery,Required] string username,[FromQuery,Required] string tiny, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
        {
            PagedData<UserClick> pagedData = await userClickService.UserClicksByTinyUrl(username, tiny, pageNum, pageSize);
            return Ok(pagedData);
        }

        [HttpGet("between")]
        public async Task<ActionResult<PagedData<UserClick>>> GetBetween([FromQuery, Required] string username,
            [FromQuery, Required] string tiny,
            [FromQuery, Required] string from,
            [FromQuery, Required] string to,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10)
        {
            PagedData<UserClick> pagedData = await userClickService.ClicksBetweenDates(username, from, to, tiny, pageNum, pageSize);
            return Ok(pagedData);
        }
    }
}
