using FluentValidation;
using WebAPI.Models.Dtos.WarehousesDtos;

namespace WebAPI.Models.Dtos.AuthDtos;

public class CreatedWarehouseDtoValidator : AbstractValidator<CreatedWarehouseDto>
{
    public CreatedWarehouseDtoValidator()
    {
        RuleFor(c => c.Capacity).GreaterThan(0);
        RuleFor(c => c.SetupCost).NotEmpty();
    }
}