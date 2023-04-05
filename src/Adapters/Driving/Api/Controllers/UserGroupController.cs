using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UserGroupController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IGroupListingSLService _groupListingSLService;
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IMapper mapper, IGroupListingSLService groupListingSLService, IUserGroupService userGroupService)
        {
            _mapper = mapper;
            _groupListingSLService = groupListingSLService;
            _userGroupService = userGroupService;
        }


        [HttpGet("group-listing")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GroupListingViewModel>>> GroupListing(int? bplId)
        {
            try
            {
                var gl = await _groupListingSLService.GetGroupListing(bplId);

                return Ok(_mapper.Map<IEnumerable<GroupListingViewModel>>(gl));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("group-listing/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupListingViewModel>> GroupListing(string code)
        {
            try
            {
                var gl = await _groupListingSLService.GetGroupListing(code);

                if (gl == null)
                    return NotFound();

                return Ok(_mapper.Map<GroupListingViewModel>(gl));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPost("group-listing")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateGroupListingViewModel>> CreateGroupListing(CreateGroupListingViewModel createGL, int bplId)
        {
            try
            {
                var gl = await _groupListingSLService.CreateGroupListing(createGL.Code, createGL.Name, bplId);

                return Created("", _mapper.Map<CreateGroupListingViewModel>(gl));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpDelete("group-listing/{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateGroupListingViewModel>> DeleteGroupListing(string code)
        {
            try
            {
                if (await _groupListingSLService.DeleteGroupListing(code) == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPatch("add-user-group-listing/{groupId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddUserGroupListing(string groupId, IEnumerable<UserViewModel> users)
        {
            try
            {
                if (await _groupListingSLService.AddUserGroupListingAsync(groupId, _mapper.Map<IEnumerable<User>>(users)) == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPatch("remove-user-group-listing/{groupId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUserGroupListing(string groupId, IEnumerable<UserViewModel> users)
        {
            try
            {
                if (await _userGroupService.DeleteUserGroupListingAsync(groupId, _mapper.Map<IEnumerable<User>>(users)) == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
    }
}