using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces
{
    public interface ITagCorreiosSLService
    {
        Task<SroData?> GetCurrent(string method, int tryLogin = 0);
        Task SetCurrent(string Code, int Current, int tryLogin = 0);
        Task SetFinished(string Code, int tryLogin = 0);
    }
}