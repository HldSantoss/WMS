using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces;

public interface IBranchesSLService
{
    Task<Branches> GetBranch(int bplId, int tryLogin = 0);
    Task<Branches> GetAllBranch(int tryLogin = 0);
}