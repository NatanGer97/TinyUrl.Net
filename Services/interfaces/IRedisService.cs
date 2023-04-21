using TinyUrl.Models;

namespace TinyUrl.Services.interfaces
{
    public interface IRedisService
    {
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
