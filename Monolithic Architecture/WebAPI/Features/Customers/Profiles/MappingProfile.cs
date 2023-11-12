using AutoMapper;
using BusinessLayer.Features.Customers.Commands.CreateCustomer;
using BusinessLayer.Features.Customers.Commands.DeleteCustomer;
using BusinessLayer.Features.Customers.Commands.UpdateCustomer;
using BusinessLayer.Features.Customers.Dtos;
using BusinessLayer.Features.Customers.Models;
using WebAPI.Persistence.Paging;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.Customers.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
        CreateMap<Customer, DeleteCustomerCommand>().ReverseMap();
        CreateMap<Customer, UpdateCustomerCommand>().ReverseMap();
        CreateMap<Customer, CreatedCustomerDto>().ReverseMap();
        CreateMap<Customer, DeletedCustomerDto>().ReverseMap();
        CreateMap<Customer, UpdatedCustomerDto>().ReverseMap();
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<Customer, CustomerListDto>().ReverseMap();
        CreateMap<IPaginate<Customer>, CustomerListModel>().ReverseMap();
    }
}
