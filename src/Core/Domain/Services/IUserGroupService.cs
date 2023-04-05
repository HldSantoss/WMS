using Domain.Entities;

namespace Domain.Services
{
    public interface IUserGroupService
    {
        Task<string?> DeleteUserGroupListingAsync(string code, IEnumerable<User> users);
    }
}