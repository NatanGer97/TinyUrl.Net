using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Responses;
using TinyUrl.Services;
using TinyUrl.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IUrlService urlService;
        public UrlController(UserService userService, IUrlService urlService)
        {
            this.userService = userService;
            this.urlService = urlService;            
        }
        

        [HttpPost("createUrl")]
        public async Task<ActionResult> CreateUrl([FromBody] NewTinyUrlReq newTinyUrlReq)
        {

            User? user = await userService.FindUserByUserNameAsync(newTinyUrlReq.Username);

            if (user == null)
            {
                return NotFound("user with username:" + newTinyUrlReq.Username + "not found");
            }

            if (newTinyUrlReq.Url == null) return BadRequest();

            string createdTinyUrl = urlService.CreateNewTinyUrl(newTinyUrlReq);
            
            if (createdTinyUrl != null || createdTinyUrl != string.Empty)
            {
                return Created(string.Empty, createdTinyUrl);
            }

            
            return BadRequest();
        }

    }
}
