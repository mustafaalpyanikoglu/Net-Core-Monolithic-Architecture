using AutoMapper;
using BusinessLayer.Features.Warehouses.Models;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Application.Requests;
using WebAPI.Persistence.Paging;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using MediatR;
using static BusinessLayer.Features.Warehouses.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.Warehouses.Queries.GetListWarehouse;

public class GetListWarehouseQuery : IRequest<WarehouseListModel>//, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public string[] Roles => new[] { ADMIN, WarehouseGet };

    public class GetListWarehouseQueryHanlder : IRequestHandler<GetListWarehouseQuery, WarehouseListModel>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public GetListWarehouseQueryHanlder(IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<WarehouseListModel> Handle(GetListWarehouseQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Warehouse> warehouses = await _warehouseRepository.GetListAsync(
                index: request.PageRequest.Page,
                size: request.PageRequest.PageSize);

            WarehouseListModel mappedWarehouseListModel = _mapper.Map<WarehouseListModel>(warehouses);
            return mappedWarehouseListModel;
        }
    }
}
