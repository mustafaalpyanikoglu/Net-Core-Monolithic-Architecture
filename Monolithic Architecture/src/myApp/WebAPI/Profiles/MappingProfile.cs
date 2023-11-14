using AutoMapper;
using WebAPI.Models.Concrete;
using WebAPI.Models.Dtos.CustomerDtos;
using WebAPI.Models.Dtos.CustomerWarehouseCostsDtos;
using WebAPI.Models.Dtos.UserDtos;
using WebAPI.Models.Dtos.WarehousesDtos;

namespace WebAPI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Customer Mapping
        CreateMap<Customer, CreatedCustomerDto>().ReverseMap();
        CreateMap<Customer, DeletedCustomerDto>().ReverseMap();
        CreateMap<Customer, UpdatedCustomerDto>().ReverseMap();
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<Customer, CustomerListDto>().ReverseMap();
        #endregion

        #region Customer Warehouse Cost Mapping
        CreateMap<CustomerWarehouseCost, CreatedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, DeletedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, UpdatedCustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, CustomerWarehouseCostDto>().ReverseMap();
        CreateMap<CustomerWarehouseCost, CustomerWarehouseCostListDto>().ReverseMap();
        #endregion

        #region User Mapping
        CreateMap<User, CreatedUserDto>().ReverseMap();
        CreateMap<User, DeletedUserDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserListDto>().ReverseMap();
        #endregion

        #region Warehouse Mapping
        CreateMap<Warehouse, CreatedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, DeletedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, UpdatedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, WarehouseDto>().ReverseMap();
        CreateMap<Warehouse, WarehouseListDto>().ReverseMap();
        #endregion
    }
}
