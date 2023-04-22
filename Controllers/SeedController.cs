using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TinyUrl.Models.Enums;
using TinyUrl.Models;
using TinyUrl.Services;
using System.ComponentModel.DataAnnotations;
using TinyUrl.Services.interfaces;
using TinyUrl.Models.Dto;

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        /* public async Task OnUrlClickAsync(UserClick userClick)
         {
             string tinyUrl = userClick.TinyUrl;
             string username = userClick.Username;
             // increment user clicks
             await userService.IncrementClickField(username, string.Empty, eKeys.UserClicks);
             await userService.IncrementClickField(username, tinyUrl, eKeys.UserTinyUrlsClicksMonth);
             // save new click instacne
             await userClickService.AddNewClickAsync(userClick);
         }*/

        private readonly IUrlService urlService;
        private readonly UserService userService;
        private readonly IUserClickService userClickService;
        private readonly UrlController urlController;
        public SeedController(IUrlService urlService, UserService userService, IUserClickService userClickService, UrlController urlController)
        {
            this.urlService = urlService;
            this.userService = userService;
            this.userClickService = userClickService;
            this.urlController = urlController;
            
        }

       

        [HttpPost("seed_user_clicks")]
        public async Task<ActionResult> seedUserClicks([FromQuery, EmailAddress] string username)
        {
            /*
             *         string tinyUrl = userClick.TinyUrl;
             string username = userClick.Username;
             // increment user clicks
             await userService.IncrementClickField(username, string.Empty, eKeys.UserClicks);
             await userService.IncrementClickField(username, tinyUrl, eKeys.UserTinyUrlsClicksMonth);
             // save new click instacne
             await userClickService.AddNewClickAsync(userClick);
             */
            // get from mongo all tiny clicks
            List<TinyUrlInDB> tinyUrlsInDB = await urlService.GetAllTinyCodesByUsernameAsync(username);
            List<UserClick> userClicks = new List<UserClick>();
            foreach (TinyUrlInDB tinyUrlDocument in tinyUrlsInDB)
            {
                int counter = 0;
                while (counter++ < 5)
                {

                    UserClick userClick = createNewUserClick(tinyUrlDocument);
                    userClicks.Add(userClick);
                    await userClickService.AddNewClickAsync(userClick);
                    await userService.IncrementClickField(username, string.Empty, eKeys.UserClicks);
                    await userService.IncrementClickFieldWithGivenDate(username, tinyUrlDocument.Tiny, userClick.ClickedAt);

                }



               

            }


            return Ok(userClicks);
        }

        private UserClick createNewUserClick(TinyUrlInDB tinyUrlDocument)
        {
            Random random = new Random();
            DateTime randDateTime = DateTime.UtcNow
                .AddMinutes(random.Next(5, 50))
                .AddHours(random.Next(1, 11))
                .AddMonths(random.Next(1, 12));

            UserClick userClick = new UserClick()
            {
                TinyUrl = tinyUrlDocument.Tiny,
                OriginalUrl = tinyUrlDocument.OriginalUrl,
                Username = tinyUrlDocument.Username,
                ClickedAt = randDateTime
            };

            return userClick;
        }
    }
}
