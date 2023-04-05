using System;
using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces
{
	public interface IGroupListingSLService
	{
        Task<IEnumerable<GroupListing>> GetGroupListing(int? bplId = null, int tryLogin = 0);
        Task<GroupListing?> GetGroupListing(string code, int tryLogin = 0);
        Task<GroupListing> CreateGroupListing(string code, string name, int? bplId = null, int tryLogin = 0);
        Task<string?> DeleteGroupListing(string code, int tryLogin = 0);
        Task<string?> AddUserGroupListingAsync(string code, IEnumerable<User> usersCode, int tryLogin = 0);
        Task DeleteUserGroupListingAsync(string code, string bplId, List<User> usersStay, int tryLogin = 0);
    }
}

