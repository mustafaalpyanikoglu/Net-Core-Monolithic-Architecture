using AutoMapper;
using WebAPI.Persistence.Paging;
using BusinessLayer.Features.Warehouses.Dtos;
using BusinessLayer.Features.Warehouses.Models;
using BusinessLayer.Features.Warehouses.Commands.CreateWarehouse;
using BusinessLayer.Features.Warehouses.Commands.DeleteWarehouse;
using BusinessLayer.Features.Warehouses.Commands.UpdateWarehouse;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Warehouses.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Warehouse, CreateWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, DeleteWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, UpdateWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, CreatedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, DeletedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, UpdatedWarehouseDto>().ReverseMap();
        CreateMap<Warehouse, WarehouseDto>().ReverseMap();
        CreateMap<Warehouse, WarehouseListDto>().ReverseMap();
        CreateMap<IPaginate<Warehouse>, WarehouseListModel>().ReverseMap();
    }
}
