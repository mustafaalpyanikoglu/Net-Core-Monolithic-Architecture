using BusinessLayer.Features.OperationClaims.Constants;
using WebAPI.Application.Rules;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Warehouses.Rules;

public class WarehouseBusinessRules : BaseBusinessRules
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseBusinessRules(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task WarehouseIdShouldExistWhenSelected(int? id)
    {
        Warehouse? result = await _warehouseRepository.GetAsync(b => b.Id == id);
        if (result == null) throw new BusinessException(OperationClaimMessages.OperationClaimNotFound);
    }

}
