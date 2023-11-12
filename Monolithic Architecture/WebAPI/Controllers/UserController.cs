using BusinessLayer.Features.Users.Command.ResetUserImage;
using BusinessLayer.Features.Users.Command.UpdateUserCV;
using BusinessLayer.Features.Users.Command.UpdateUserImage;
using BusinessLayer.Features.Users.Models;
using BusinessLayer.Features.Users.Queries.GetByIdUser;
using BusinessLayer.Features.Users.Command.CreateUser;
using BusinessLayer.Features.Users.Command.DeleteUser;
using BusinessLayer.Features.Users.Command.UpdateUser;
using BusinessLayer.Features.Users.Dtos;
using BusinessLayer.Features.Users.Queries.GetListUser;
using BusinessLayer.Features.Users.Queries.GetListUserByDynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BusinessLayer.Features.Users.Queries.GetByTokenUser;
using WebAPI.Models.Constants;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.Application.Requests;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        [ProducesResponseType(typeof(CreatedUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("add")] 
        public async Task<IActionResult> Add([FromBody] CreateUserCommand createUserCommand)
        {
            CreatedUserDto result = await Mediator.Send(createUserCommand);
            return Created("", result);
        }

        [ProducesResponseType(typeof(DeletedUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserCommand deleteUserCommand)
        {
            DeletedUserDto result = await Mediator.Send(deleteUserCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UpdatedUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] UpdateUserCommand updateUserCommand)
        {
            UpdatedUserResponseDto result = await Mediator.Send(updateUserCommand);
            return Ok(result);
        }


        [ProducesResponseType(typeof(UpdatedUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("updateuserimage")]
        public async Task<IActionResult> UpdateUserImage([FromForm] UpdateUserImageCommand updateUserImageCommand)
        {
            UpdatedUserResponseDto result = await Mediator.Send(updateUserImageCommand);
            return Ok(result);
        }


        [ProducesResponseType(typeof(ResetUserImageRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("resetuserimage")]
        public async Task<IActionResult> ResetUserImage([FromBody] ResetUserImageCommand resetUserImageCommand)
        {
            ResetUserImageRequestDto resetUserImageRequestDto = await Mediator.Send(resetUserImageCommand);
            return Ok(resetUserImageRequestDto);
        }

        [ProducesResponseType(typeof(UpdatedUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("updateusercv")]
        public async Task<IActionResult> UpdateUserCV([FromForm] UpdateUserCVCommand updateUserCVCommand)
        {
            UpdatedUserResponseDto result = await Mediator.Send(updateUserCVCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserListModel), StatusCodes.Status200OK)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListUserQuery getListUserQuery = new() { PageRequest = pageRequest };
            UserListModel result = await Mediator.Send(getListUserQuery);
            return Ok(result);
        }

        //[ProducesResponseType(typeof(UpdatedUserFromAuthDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        //[SwaggerOperation(description: ResponseDescriptions.AUTH_REGISTER)]
        //[HttpPost("fromauth")]
        //public async Task<IActionResult> UpdateFromAuth([FromBody] UpdateUserFromAuthCommand updateUserFromAuthCommand)
        //{
        //    UpdatedUserFromAuthDto result = await Mediator.Send(updateUserFromAuthCommand);
        //    return Ok(result);
        //}

        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            GetByIdUserQuery getByIdUserQuery = new() { Id = id };
            UserDto result = await Mediator.Send(getByIdUserQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbytoken")]
        public async Task<IActionResult> GetByToken()
        {
            GetByTokenUserQuery getByTokenUserQuery = new() { };
            UserDto result = await Mediator.Send(getByTokenUserQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDynamicResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("getlist/bydynamic")]
        public async Task<IActionResult> GetListByDynamic([FromBody] QueryModel queryModel)
        {
            GetListUserByDynamicQuery listUsersByDynamicQuery = new() { PageRequest = queryModel.PageRequest, Dynamic = queryModel.DynamicQuery};
            UserListModel result = await Mediator.Send(listUsersByDynamicQuery);
            return Ok(result);
        }
    }
}
