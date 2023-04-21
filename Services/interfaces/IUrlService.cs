using TinyUrl.Models;
using TinyUrl.Models.Dto;

namespace TinyUrl.Services.interfaces
{
    public interface IUrlService
    {
        /// <summary>
        /// create new mapping between original url to shorter url
        /// </summary>
        /// <param name="newTinyUrlReq"></param>
        /// <returns>return tiny url</returns>
        /// <exception cref="InternalServerException"></exception>
        /// <exception cref="InternalServerException"></exception>
        Task<string> CreateNewTinyUrlAsync(NewTinyUrlReq newTinyUrlReq);

        /// <summary>
        /// find and return tinyurl obj fitting to to given code
        /// </summary>
        /// <param name="tinycode"></param>
        /// <returns>TinyUrlFromRedis</returns>
        /// <exception cref="NotFoundException">if the tiny code not exsit in redis</exception
        TinyUrlFromRedis? GetTinyUrlObjByCode(string tinycode);

        /// <summary>
        /// check if the tiny code exist in redis
        /// </summary>
        /// <param name="tinycode"></param>
        /// <returns> true if exist or false if is not</returns>
         bool isTinyCodeExist(string tinycode);


    }
}
