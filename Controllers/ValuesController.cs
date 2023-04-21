using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public IndexController(UserService userService, IUrlService urlService, ILogger<IndexController> logger)
        {
            this.userService = userService;
            this.urlService = urlService;
            this.logger = logger;
        }

        [HttpGet("{tiny}")]
        public async Task<ActionResult> RedirectToOrginalUrlAsync([FromRoute] string tiny)
        {
            TinyUrlFromRedis? tinyUrlFromRedis = urlService.GetTinyUrlObjByCode(tiny);

            if (tinyUrlFromRedis != null)
            {
                string url = tinyUrlFromRedis.Url;
                logger.LogInformation("Redirecting to -> " + url);
                // on click do
                await userService.IncrementMongoField(tinyUrlFromRedis.Username, "UserClicks");
                await userService.IncrementMongoField(tinyUrlFromRedis.Username, "shorts_" + tiny + "_clicks_" + DateTime.UtcNow.ToString("MM/yyyy"));
                
                return Redirect(url);
            }

            return NotFound();
        }
    }
}
