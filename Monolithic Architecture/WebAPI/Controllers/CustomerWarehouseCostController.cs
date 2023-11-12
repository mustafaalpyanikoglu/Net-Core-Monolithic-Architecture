using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using WebAPI.Application.Requests;
using WebAPI.CrossCuttingConcerns;
using WebAPI.Models.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.DeleteCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.CreateCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.UpdateCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Models;
using BusinessLayer.Features.CustomerWarehouseCosts.Queries.GetListCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Queries.GetByIdCustomerWarehouseCosts;
using WebAPI.CrossCuttingConcerns.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerWarehouseCostController : BaseController
    {
        [ProducesResponseType(typeof(CreatedCustomerWarehouseCostDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("add")] 
        public async Task<IActionResult> Add([FromBody] CreateCustomerWarehouseCostCommand createCustomerWarehouseCostCommand)
        {
            CreatedCustomerWarehouseCostDto result = await Mediator.Send(createCustomerWarehouseCostCommand);
            return Created("", result);
        }

        [ProducesResponseType(typeof(DeletedCustomerWarehouseCostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerWarehouseCostCommand deleteCustomerWarehouseCostCommand)
        {
            DeletedCustomerWarehouseCostDto result = await Mediator.Send(deleteCustomerWarehouseCostCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UpdatedCustomerWarehouseCostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] UpdateCustomerWarehouseCostCommand updateCustomerWarehouseCostCommand)
        {
            UpdatedCustomerWarehouseCostDto result = await Mediator.Send(updateCustomerWarehouseCostCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CustomerWarehouseCostListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerWarehouseCostListModel), StatusCodes.Status200OK)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListCustomerWarehouseCostQuery getListCustomerWarehouseCostQuery = new() { PageRequest = pageRequest };
            CustomerWarehouseCostListModel result = await Mediator.Send(getListCustomerWarehouseCostQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CustomerWarehouseCostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            GetByIdCustomerWarehouseCostQuery getByIdCustomerWarehouseCostQuery = new() { Id = id };
            CustomerWarehouseCostDto result = await Mediator.Send(getByIdCustomerWarehouseCostQuery);
            return Ok(result);
        }
    }
}
