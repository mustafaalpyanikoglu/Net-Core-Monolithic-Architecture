using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contexts;
using WebAPI.CrossCuttingConcerns.Exceptions;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Models.Dtos.AuthDtos;
using WebAPI.Models.Dtos.WarehousesDtos;
using WebAPI.Security.Constants;
using WebAPI.Security.Jwt;
using static WebAPI.Models.Constants.ResponseDescriptions;
using static WebAPI.Models.Constants.OperationClaims;
using WebAPI.Security.SecurityExtensions;
using WebAPI.CrossCuttingConcerns.Exceptions.HttpProblemDetails;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IValidator<CreatedWarehouseDto> _createdWarehouseDtoValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WarehouseController(
            IMapper mapper, 
            IValidator<CreatedWarehouseDto> createdWarehouseDtoValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _createdWarehouseDtoValidator = createdWarehouseDtoValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        [ProducesResponseType(typeof(CreatedWarehouseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: EXCEPTION_DETAIL)]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreatedWarehouseDto createdWarehouseDto)
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

                ValidationContext<CreatedWarehouseDto> validatorContext = new ValidationContext<CreatedWarehouseDto>(createdWarehouseDto);
                IEnumerable<ValidationExceptionModel> errors = _createdWarehouseDtoValidator.Validate(validatorContext)
                    .Errors
                    .Where(failure => failure != null)
                    .GroupBy(
                        keySelector: p => p.PropertyName,
                        resultSelector: (propertyName, errors) =>
                            new ValidationExceptionModel { Property = propertyName, Errors = errors.Select(e => e.ErrorMessage) }
                    )
                    .ToList();
                if (errors.Any()) throw new WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException(errors);

                // database operation
                using (var context = new BaseDbContext())
                {
                    Warehouse? warehouse = _mapper.Map<Warehouse>(createdWarehouseDto);
                    EntityEntry<Warehouse> result = await context.Warehouses.AddAsync(warehouse);
                    await context.SaveChangesAsync();

                    return Created("", warehouse);
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

            catch (BusinessException ex)
            {
                return BadRequest(new ErrorModel()
                {
                    Type = BUSINESS_ERROR_TYPE,
                    Title = BUSINESS_ERROR_TITLE,
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = BUSINESS_ERROR_INSTANCE
                });
            }

            catch (WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException ex)
            {
                return BadRequest(new ErrorModel()
                {
                    Failures = ex.Errors.Select(validationException =>
                    new Failure
                    {
                        Property = validationException.Property ?? string.Empty,
                        Errors = validationException.Errors?.ToList() ?? new List<string>()
                    }).ToList(),
                    Type = VALIDATION_ERROR_TYPE,
                    Title = VALIDATION_ERROR_TITLE,
                    Detail = VALIDATION_ERROR_DETAIL,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = VALIDATION_ERROR_INSTANCE
                });
            }
            catch (Exception ex)
            {
                // Diğer hata durumları...
                return StatusCode(500, new { message = SERVER_ERROR });
            }
        }


        [ProducesResponseType(typeof(DeletedWarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: EXCEPTION_DETAIL)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeletedWarehouseDto deletedWarehouseDto)
        {
            try
            {
                // authorization-authentication control
                string[] Roles = new[] { ADMIN, USER };
                List<string>? userRoleClaims = _httpContextAccessor.HttpContext?.User.ClaimRoles();
                if (userRoleClaims is null)
                    throw new AuthorizationException(NOT_AUTHENTICATED);

                bool isNotMatchedAUserRoleClaimWithRequestRoles = string.IsNullOrEmpty(
                    userRoleClaims
                        .FirstOrDefault(
                            userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || Roles.Any(role => role == userRoleClaim)
                        ));

                if (isNotMatchedAUserRoleClaimWithRequestRoles)
                    throw new AuthorizationException(NOT_AUTHORIZED);

                // database operation
                using (var context = new BaseDbContext())
                {
                    Warehouse? warehouse = await context.Warehouses.FindAsync(deletedWarehouseDto.Id);
                    if (warehouse is null) throw new BusinessException(WAREHOUSE_NOT_FOUND);

                    context.Entry(warehouse).State = EntityState.Deleted;
                    await context.SaveChangesAsync();

                    return Ok(deletedWarehouseDto);
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

            catch (BusinessException ex)
            {
                return BadRequest(new ErrorModel()
                {
                    Type = BUSINESS_ERROR_TYPE,
                    Title = BUSINESS_ERROR_TITLE,
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = BUSINESS_ERROR_INSTANCE
                });
            }
            catch (Exception ex)
            {
                // Diğer hata durumları...
                return StatusCode(500, new { message = SERVER_ERROR });
            }
        }

        [ProducesResponseType(typeof(UpdatedWarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: EXCEPTION_DETAIL)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdatedWarehouseDto updatedWarehouseDto)
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
                    Warehouse? warehouse = await context.Warehouses.FindAsync(updatedWarehouseDto.Id);
                    if (warehouse is null) throw new BusinessException(WAREHOUSE_NOT_FOUND);

                    warehouse = _mapper.Map<Warehouse>(updatedWarehouseDto);
                    context.Entry(warehouse).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    return Created("", warehouse);
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
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorModel()
                {
                    Type = BUSINESS_ERROR_TYPE,
                    Title = BUSINESS_ERROR_TITLE,
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = BUSINESS_ERROR_INSTANCE
                });
            }
            catch (Exception ex)
            {
                // Diğer hata durumları...
                return StatusCode(500, new { message = SERVER_ERROR });
            }
        }


        [ProducesResponseType(typeof(List<Warehouse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Warehouse>), StatusCodes.Status200OK)]
        [SwaggerOperation(description: EXCEPTION_DETAIL)]
        [HttpGet("getlist")]
        public async Task<IActionResult> GetList()
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
                    List<Warehouse>? warehouse = await context.Warehouses.AsNoTracking().ToListAsync();
                    return Ok(warehouse);
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

        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(description: EXCEPTION_DETAIL)]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
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
                    Warehouse? warehouse = await context.Warehouses.AsNoTracking().FirstAsync(t => t.Id == id);
                    return Ok(warehouse);
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
    }
}
