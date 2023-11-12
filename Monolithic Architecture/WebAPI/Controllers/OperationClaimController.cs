using BusinessLayer.Features.OperationClaims.Commands.CreateOperationClaim;
using BusinessLayer.Features.OperationClaims.Commands.DeleteOperationClaim;
using BusinessLayer.Features.OperationClaims.Commands.UpdateOperationClaim;
using BusinessLayer.Features.OperationClaims.Models;
using BusinessLayer.Features.OperationClaims.Queries.GetByIdOperationClaim;
using BusinessLayer.Features.OperationClaims.Queries.GetListOperationClaim;
using BusinessLayer.Features.OperationClaims.Queries.GetListOperationClaimByDynamic;
using WebAPI.Application.Requests;
using WebAPI.Models.Constants;
using WebAPI.Models.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Features.OperationClaims.Dtos;
using WebAPI.CrossCuttingConcerns.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimController : BaseController
    {

        [ProducesResponseType(typeof(CreatedOperationClaimDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateOperationClaimCommand createOperationClaimCommand)
        {
            CreatedOperationClaimDto result = await Mediator.Send(createOperationClaimCommand);
            return Created("", result);
        }

        [ProducesResponseType(typeof(DeletedOperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteOperationClaimCommand deleteOperationClaimCommand)
        {
            DeletedOperationClaimDto result = await Mediator.Send(deleteOperationClaimCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UpdatedOperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateOperationClaimCommand updateOperationClaimCommand)
        {
            UpdatedOperationClaimDto result = await Mediator.Send(updateOperationClaimCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(OperationClaimListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListOperationClaimQuery getListOperationClaimQuery = new() { PageRequest = pageRequest };
            OperationClaimListModel result = await Mediator.Send(getListOperationClaimQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(OperationClaimDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            GetByIdOperationClaimQuery getByIdOperationClaimQuery = new() { Id = id };
            OperationClaimDto result = await Mediator.Send(getByIdOperationClaimQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(OperationClaimListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDynamicResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("getlist/bydynamic")]
        public async Task<IActionResult> GetListByDynamic([FromBody] QueryModel queryModel)
        {
            GetListOperationClaimByDynamicQuery getListOperationClaimByDynamicQuery = new() { PageRequest = queryModel.PageRequest, Dynamic = queryModel.DynamicQuery };
            OperationClaimListModel result = await Mediator.Send(getListOperationClaimByDynamicQuery);
            return Ok(result);
        }
    }
}
