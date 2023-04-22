using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TinyUrl.Models.Enums;
using TinyUrl.Models;
using TinyUrl.Services;
using System.ComponentModel.DataAnnotations;
using TinyUrl.Services.interfaces;
using TinyUrl.Models.Dto;
using MongoDB.Driver.Linq;

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IUrlService urlService;
        private readonly UserService userService;
        private readonly IUserClickService userClickService;
        public SeedController(IUrlService urlService, UserService userService, IUserClickService userClickService)
        {
            this.urlService = urlService;
            this.userService = userService;
            this.userClickService = userClickService;
            
        }
       

        [HttpPost("seed_user_clicks")]
        public async Task<ActionResult> seedUserClicks([FromQuery, EmailAddress] string username)
        {

            List<TinyUrlInDB> tinyUrlsInDB = await urlService.GetAllTinyCodesByUsernameAsync(username);
            List<UserClick> userClicks = new List<UserClick>();
            foreach (TinyUrlInDB tinyUrlDocument in tinyUrlsInDB)
            {
                int counter = 0;
                while (counter++ < 100)
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
            int month = random.Next(1, 13);
            int year = random.Next(2019, 2024);
            var currentDateTime = DateTime.UtcNow;
            currentDateTime = currentDateTime.AddHours(random.Next(1, 11));
            currentDateTime = currentDateTime.AddMinutes(random.Next(5, 50));
            DateTime randDateTime = new DateTime(year, month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second).ToUniversalTime();

   
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
