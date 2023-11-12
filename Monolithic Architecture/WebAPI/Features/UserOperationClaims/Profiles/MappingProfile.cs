using AutoMapper;
using BusinessLayer.Features.UserOperationClaims.Commands.CreateUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Commands.DeleteUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Commands.UpdateUserOperationClaim;
using BusinessLayer.Features.UserOperationClaims.Dtos;
using BusinessLayer.Features.UserOperationClaims.Models;
using WebAPI.Persistence.Paging;
using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.UserOperationClaims.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserOperationClaim, CreateUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim, DeleteUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim, UpdateUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim, CreateUserOperationClaimDto>().ReverseMap();
        CreateMap<UserOperationClaim, DeleteUserOperationClaimDto>().ReverseMap();
        CreateMap<UserOperationClaim, UpdateUserOperationClaimDto>().ReverseMap();
        CreateMap<UserOperationClaim, UserOperationClaimDto>().ForMember(t => t.UserFirstName, opt => opt.MapFrom(u => u.User.FirstName))
                                                                  .ForMember(t => t.UserLastName, opt => opt.MapFrom(u => u.User.LastName))
                                                                  .ForMember(t => t.OperationClaimName, opt => opt.MapFrom(u => u.OperationClaim.Name))
                                                                  .ForMember(t => t.OperationClaimNameDescription, opt => opt.MapFrom(u => u.OperationClaim.Description))
                                                                  .ReverseMap();
        CreateMap<UserOperationClaim, UserOperationClaimListDto>().ForMember(t => t.UserFirstName, opt => opt.MapFrom(u => u.User.FirstName))
                                                                  .ForMember(t => t.UserLastName, opt => opt.MapFrom(u => u.User.LastName))
                                                                  .ForMember(t => t.OperationClaimName, opt => opt.MapFrom(u => u.OperationClaim.Name))
                                                                  .ForMember(t => t.OperationClaimNameDescription, opt => opt.MapFrom(u => u.OperationClaim.Description))
                                                                  .ReverseMap();
        CreateMap<IPaginate<UserOperationClaim>, UserOperationClaimListModel>().ReverseMap();
    }
}
