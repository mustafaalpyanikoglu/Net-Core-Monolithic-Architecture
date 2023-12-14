using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contexts;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Models.Dtos.UserDtos;
using WebAPI.Security.Constants;
using static WebAPI.Models.Constants.ResponseDescriptions;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Security.SecurityExtensions;
using WebAPI.Paging;
using System.Drawing;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [ProducesResponseType(typeof(UserListModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserListModel), StatusCodes.Status200OK)]
    [SwaggerOperation(description: EXCEPTION_DETAIL)]
    [HttpGet("getlist")]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        try
        {
            // authorization-authentication control
            string[] Roles = new[] { ADMIN, USER };
            List<string>? userRoleClaims = _httpContextAccessor.HttpContext?.User.ClaimRoles();
            if (userRoleClaims == null)
                throw new AuthorizationException(NOT_AUTHENTICATED);
            bool isNotMatchedAUserRoleClaimWithRequestRoles = string.IsNullOrEmpty(
                userRoleClaims
                    .FirstOrDefault(
                        userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || Roles.Any(role => role == userRoleClaim)
                    ));
            if (isNotMatchedAUserRoleClaimWithRequestRoles)
                throw new AuthorizationException(NOT_AUTHORIZED);

            // database operation
            using (BaseDbContext context = new BaseDbContext())
            {
                IQueryable<User> queryable = context.Users.AsNoTracking();
                IPaginate<User> users = await queryable.ToPaginateAsync(pageRequest.Page, pageRequest.PageSize);
                return Ok(users);
            }
        }
        catch (AuthorizationException exception)
        {
            return Unauthorized(new ErrorModel()
            {
                Title = AUTHORIZATION_ERROR_TITLE,
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized,
                Type = AUTHORIZATION_ERROR_TYPE,
                Instance = AUTHORIZATION_ERROR_INSTANCE
            });
        }
        catch (Exception ex)
        {
            // Diğer hata durumları...
            return StatusCode(500, new { message = SERVER_ERROR });
        }
    }

    //[ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    //[SwaggerOperation(description: EXCEPTION_DETAIL)]
    //[HttpGet("getlist")]
    //public async Task<IActionResult> GetList()
    //{
    //    try
    //    {
    //        // authorization-authentication control
    //        string[] Roles = new[] { ADMIN, USER };
    //        List<string>? userRoleClaims = _httpContextAccessor.HttpContext?.User.ClaimRoles();
    //        if (userRoleClaims == null)
    //            throw new AuthorizationException(NOT_AUTHENTICATED);
    //        bool isNotMatchedAUserRoleClaimWithRequestRoles = string.IsNullOrEmpty(
    //            userRoleClaims
    //                .FirstOrDefault(
    //                    userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || Roles.Any(role => role == userRoleClaim)
    //                ));
    //        if (isNotMatchedAUserRoleClaimWithRequestRoles)
    //            throw new AuthorizationException(NOT_AUTHORIZED);

    //        // database operation
    //        using (BaseDbContext context = new BaseDbContext())
    //        {
    //            List<User>? users = await context.Users.AsNoTracking().ToListAsync();
    //            return Ok(users);
    //        }
    //    }
    //    catch (AuthorizationException exception)
    //    {
    //        return Unauthorized(new ErrorModel()
    //        {
    //            Title = AUTHORIZATION_ERROR_TITLE,
    //            Detail = exception.Message,
    //            Status = StatusCodes.Status401Unauthorized,
    //            Type = AUTHORIZATION_ERROR_TYPE,
    //            Instance = AUTHORIZATION_ERROR_INSTANCE
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        // Diğer hata durumları...
    //        return StatusCode(500, new { message = SERVER_ERROR });
    //    }
    //}
}
