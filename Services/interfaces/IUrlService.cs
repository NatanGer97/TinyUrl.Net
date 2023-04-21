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




    }
}
