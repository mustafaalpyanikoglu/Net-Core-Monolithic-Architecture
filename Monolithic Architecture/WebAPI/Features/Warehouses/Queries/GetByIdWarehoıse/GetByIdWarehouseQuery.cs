using AutoMapper;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Warehouses.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;
using BusinessLayer.Features.Warehouses.Dtos;
using BusinessLayer.Features.Warehouses.Rules;
using WebAPI.Repositories.Abstract;

namespace BusinessLayer.Features.Warehouses.Queries.GetByIdWarehoıse;

public class GetByIdWarehouseQuery : IRequest<WarehouseDto>//, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { ADMIN, WarehouseGet };

    public class GetByIdWarehouseQueryHandler : IRequestHandler<GetByIdWarehouseQuery, WarehouseDto>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;
        private readonly WarehouseBusinessRules _warehouseBusinessRules;

        public GetByIdWarehouseQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper, WarehouseBusinessRules warehouseBusinessRules)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
            _warehouseBusinessRules = warehouseBusinessRules;
        }

        public async Task<WarehouseDto> Handle(GetByIdWarehouseQuery request, CancellationToken cancellationToken)
        {
            await _warehouseBusinessRules.WarehouseIdShouldExistWhenSelected(request.Id);

            Warehouse? warehouse = await _warehouseRepository.GetAsync(m => m.Id == request.Id);
            WarehouseDto warehouseDto = _mapper.Map<WarehouseDto>(warehouse);

            return warehouseDto;

        }
    }
}
