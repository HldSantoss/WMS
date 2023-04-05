using Domain.Adapters;
using Domain.Entities;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;

namespace Application.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IGroupListingSLService _groupListingSLService;

        public UserGroupService(IGroupListingSLService groupListingSLService)
        {
            _groupListingSLService = groupListingSLService;
        }

        public async Task<string?> DeleteUserGroupListingAsync(string code, IEnumerable<User> users)
        {
            var gl = await _groupListingSLService.GetGroupListing(code);

            if (gl == null)
                return null;

            var usersStay = gl.Users.ToList();
            foreach (var user in users)
            {
                var _user = gl.Users.Where(p => p.UserId == user.UserId).FirstOrDefault();

                if (_user == default)
                    continue;

                usersStay.Remove(_user);
            }

            await _groupListingSLService.DeleteUserGroupListingAsync(code, gl.Bpl.ToString(), usersStay);
            
            return "";
        }
    }
}