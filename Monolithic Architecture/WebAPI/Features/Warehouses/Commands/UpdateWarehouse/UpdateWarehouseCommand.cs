using AutoMapper;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Warehouses.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using BusinessLayer.Features.Warehouses.Dtos;
using BusinessLayer.Features.Warehouses.Rules;
using WebAPI.Repositories.Abstract;

namespace BusinessLayer.Features.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommand : IRequest<UpdatedWarehouseDto>/*, ISecuredRequest*/
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public double SetupCost { get; set; }

    public string[] Roles => new[] { ADMIN, WarehouseUpdate };

    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, UpdatedWarehouseDto>
    {
        private readonly IMapper _mapper;
        private readonly WarehouseBusinessRules _warehouseBusinessRules;
        private readonly IWarehouseRepository _warehouseRepository;

        public UpdateWarehouseCommandHandler(IMapper mapper, WarehouseBusinessRules warehouseBusinessRules, IWarehouseRepository warehouseRepository)
        {
            _mapper = mapper;
            _warehouseBusinessRules = warehouseBusinessRules;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<UpdatedWarehouseDto> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            await _warehouseBusinessRules.WarehouseIdShouldExistWhenSelected(request.Id);

            Warehouse mappedWarehouse = _mapper.Map<Warehouse>(request);
            Warehouse updateWarehouse = await _warehouseRepository.UpdateAsync(mappedWarehouse);
            UpdatedWarehouseDto updatedWarehouseDto = _mapper.Map<UpdatedWarehouseDto>(updateWarehouse);

            return updatedWarehouseDto;
        }
    }
}
