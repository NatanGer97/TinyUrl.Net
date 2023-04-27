using TinyUrl.Models;
using TinyUrl.Models.Dto;

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
        /// </summary>
        bool isTinyCodeExist(string tinycode);

        /// <summary>
        /// set value to redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns><see langword="true"/> if the string was set, <see langword="false"/> otherwise.</returns>
        bool SetValue(string key, object value);

        bool SetValueWithTTL(string tinycode, object value, TimeSpan timeSpan);
    }
}
