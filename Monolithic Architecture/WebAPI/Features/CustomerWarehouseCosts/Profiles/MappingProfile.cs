using AutoMapper;
using WebAPI.Persistence.Paging;
using WebAPI.Models.Concrete;
using BusinessLayer.Features.CustomerWarehouseCosts.Dtos;
using BusinessLayer.Features.CustomerWarehouseCosts.Models;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.CreateCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.DeleteCustomerWarehouseCosts;
using BusinessLayer.Features.CustomerWarehouseCosts.Commands.UpdateCustomerWarehouseCosts;

namespace BusinessLayer.Features.CustomerWarehouseCosts.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CustomerWarehouseCost, CreateCustomerWarehouseCostCommand>().ReverseMap();
        CreateMap<CustomerWarehouseCost, DeleteCustomerWarehouseCostCommand>().ReverseMap();
        CreateMap<CustomerWarehouseCost, UpdateCustomerWarehouseCostCommand>().ReverseMap();
        CreateMap<CustomerWarehouseCost, CreatedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, DeletedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, UpdatedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, CustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, CustomerWarehouseCostListDto>().ReverseMap();
        CreateMap<IPaginate<CustomerWarehouseCost>, CustomerWarehouseCostListModel>().ReverseMap();
    }
}
