using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Models.Dtos.UserDtos;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;

    public UserController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
    [HttpGet("getlist")]
    public async Task<IActionResult> GetList()
    {
        List<User> users;

        using (var context = new BaseDbContext())
        {
            users = await context.Users.ToListAsync();
        }

        List<UserListDto> result = _mapper.Map<List<UserListDto>>(users);
        return Ok(result);

    }
}
