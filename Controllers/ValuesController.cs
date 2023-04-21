using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpCompress.Readers;
using System.Globalization;
using System.Runtime.Serialization;
using TinyUrl.Models;
using TinyUrl.Services;
using TinyUrl.Services.interfaces;

namespace TinyUrl.Controllers
{
    
    [ApiController]
    public class IndexController : ControllerBase
    {
        

        private readonly UserService userService;
        private readonly IUrlService urlService;
        private readonly ILogger<IndexController> logger;
        private readonly IRedisService redisService;

        public IndexController(UserService userService, IUrlService urlService, IRedisService redisService,
            ILogger<IndexController> logger)
        {
            this.userService = userService;
            this.urlService = urlService;
            this.logger = logger;
            this.redisService = redisService;
        }

        [HttpGet("{tiny}")]
        public async Task<ActionResult> RedirectToOrginalUrlAsync([FromRoute] string tiny)
        {
            TinyUrlFromRedis? tinyUrlFromRedis = redisService.GetTinyUrlObjByCode(tiny);

            if (tinyUrlFromRedis != null)
            {
                string url = tinyUrlFromRedis.Url;
                logger.LogInformation("Redirecting to -> " + url);
                // on click 
                await urlService.OnUrlClickAsync(tiny, tinyUrlFromRedis.Username);
                /*await userService.IncrementClickField(tinyUrlFromRedis.Username, "UserClicks");
                await userService.IncrementClickField(tinyUrlFromRedis.Username, "shorts_" + tiny + "_clicks_" + DateTime.UtcNow.ToString("MM/yyyy"));*/

                return Redirect(url);
            }

            return NotFound();
        }
    }
}
