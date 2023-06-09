﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Responses;
using TinyUrl.Services;
using TinyUrl.Services.interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using TinyUrl.Models.Enums;
using System.Security.Policy;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TinyUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // TODO: add Dto mapping 
    
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

            string createdTinyUrl = await urlService.CreateNewTinyUrlAsync(newTinyUrlReq);
            
            if (createdTinyUrl != null || createdTinyUrl != string.Empty)
            {
                return Created(string.Empty, createdTinyUrl);
            }

            
            return BadRequest();
        }

        // TODO: not done yet
        //[HttpPost("CreateTempUrl")]
    /*    public async Task<ActionResult> CreateTempUrl([FromBody] TemporyTinyUrlRequest request,
            [FromQuery, Required] eTinyUrlTimeToLiveKey timeToLiveKey, [FromQuery, Required] int duration)
        {
            request.timeToLiveKey = timeToLiveKey;
            request.TimeToLive = duration;
            string tinyUrl = await urlService.CreateTemoraryLink(request);

            return Created(string.Empty, tinyUrl);
        }*/

        /*   [HttpPost("seed")]
           public async Task<ActionResult> SeedData()
           {
               NewTinyUrlReq newTinyUrlReq = new NewTinyUrlReq() { Url = "http://www.google.com", Username = "Natan@gmail.com" };
               ActionResult actionResult = await CreateUrl(newTinyUrlReq);

               return actionResult;
           }*/

    }
}
