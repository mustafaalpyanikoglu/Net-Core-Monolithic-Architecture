using BusinessLayer.Features.Customers.Queries.GetByIdCustomer;
using BusinessLayer.Features.Customers.Dtos;
using BusinessLayer.Features.Customers.Queries.GetListCustomer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BusinessLayer.Features.Customers.Commands.DeleteCustomer;
using BusinessLayer.Features.Customers.Commands.UpdateCustomer;
using BusinessLayer.Features.Customers.Commands.CreateCustomer;
using BusinessLayer.Features.Customers.Models;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.Models.Constants;
using WebAPI.Application.Requests;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        [ProducesResponseType(typeof(CreatedCustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("add")] 
        public async Task<IActionResult> Add([FromBody] CreateCustomerCommand createCustomerCommand)
        {
            CreatedCustomerDto result = await Mediator.Send(createCustomerCommand);
            return Created("", result);
        }

        [ProducesResponseType(typeof(DeletedCustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerCommand deleteCustomerCommand)
        {
            DeletedCustomerDto result = await Mediator.Send(deleteCustomerCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(UpdatedCustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] UpdateCustomerCommand updateCustomerCommand)
        {
            UpdatedCustomerDto result = await Mediator.Send(updateCustomerCommand);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CustomerListModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerListModel), StatusCodes.Status200OK)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListCustomerQuery getListCustomerQuery = new() { PageRequest = pageRequest };
            CustomerListModel result = await Mediator.Send(getListCustomerQuery);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            GetByIdCustomerQuery getByIdCustomerQuery = new() { Id = id };
            CustomerDto result = await Mediator.Send(getByIdCustomerQuery);
            return Ok(result);
        }
    }
}
