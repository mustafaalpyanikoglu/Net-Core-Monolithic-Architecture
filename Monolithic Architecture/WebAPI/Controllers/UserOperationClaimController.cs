using BusinessLayer.Features.UserOperationClaims.Commands.CreateUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Commands.DeleteUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Commands.UpdateUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Dtos;
using BusinessLayer.Features.UserOperationClaims.Models;
using BusinessLayer.Features.UserOperationClaims.Queries.GetByIdUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Queries.GetListUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Queries.GetListUserOperationClaimByDynamic;
using WebAPI.Application.Requests;
using WebAPI.Models.Constants;
using WebAPI.Models.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.CrossCuttingConcerns.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOperationClaimController : BaseController
    {

        [ProducesResponseType(typeof(CreateUserOperationClaimDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateUserOperationClaimCommand createUserOperationClaimCommand)
        {
            CreateUserOperationClaimDto result = await Mediator.Send(createUserOperationClaimCommand);
            return Created("", result);
        }

        [ProducesResponseType(typeof(DeleteUserOperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserOperationClaimCommand deleteUserOperationClaimCommand)
        {
            DeleteUserOperationClaimDto result = await Mediator.Send(deleteUserOperationClaimCommand);
            return Ok(result);
        }
        
        [ProducesResponseType(typeof(UpdateUserOperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserOperationClaimCommand updateUserOperationClaimCommand)
        {
            UpdateUserOperationClaimDto result = await Mediator.Send(updateUserOperationClaimCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserOperationClaimListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListUserUperationClaimQuery getListUserOperationClaimQuery = new() { PageRequest = pageRequest };
            UserOperationClaimListModel result = await Mediator.Send(getListUserOperationClaimQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserOperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            GetByIdUserOperationClaimQuery getByIdUserOperationClaimQuery = new() { Id= id };
            UserOperationClaimDto result = await Mediator.Send(getByIdUserOperationClaimQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UserOperationClaimListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDynamicResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("getlist/bydynamic")]
        public async Task<IActionResult> GetListByDynamic([FromBody] QueryModel queryModel)
        {
            GetListUserOperationClaimByDynamicQuery getListUserOperationClaimByDynamicQuery = new() { PageRequest = queryModel.PageRequest, Dynamic = queryModel.DynamicQuery };
            UserOperationClaimListModel result = await Mediator.Send(getListUserOperationClaimByDynamicQuery);
            return Ok(result);
        }
    }
}
