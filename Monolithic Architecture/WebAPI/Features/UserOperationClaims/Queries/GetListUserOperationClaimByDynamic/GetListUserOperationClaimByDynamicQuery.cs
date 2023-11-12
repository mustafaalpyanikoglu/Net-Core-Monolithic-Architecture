using AutoMapper;
using BusinessLayer.Features.UserOperationClaims.Models;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.Application.Requests;
using WebAPI.Persistence.Dynamic;
using WebAPI.Persistence.Paging;
using WebAPI.Repositories.Abstract;
using WebAPI.Models.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static BusinessLayer.Features.UserOperationClaims.Constants.OperationClaims;
using static WebAPI.Models.Constants.OperationClaims;

namespace BusinessLayer.Features.UserOperationClaims.Queries.GetListUserOperationClaimByDynamic;

public class GetListUserOperationClaimByDynamicQuery : IRequest<UserOperationClaimListModel>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery Dynamic { get; set; }
    public string[] Roles => new[] { ADMIN, UserOperationClaimGet };

    public class GetListUserOperationClaimByDynamicQueryHandler : IRequestHandler<GetListUserOperationClaimByDynamicQuery, UserOperationClaimListModel>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimDal;
        private readonly IMapper _mapper;

        public GetListUserOperationClaimByDynamicQueryHandler(IUserOperationClaimRepository userOperationClaimDal, IMapper mapper)
        {
            _userOperationClaimDal = userOperationClaimDal;
            _mapper = mapper;
        }

        public async Task<UserOperationClaimListModel> Handle(GetListUserOperationClaimByDynamicQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserOperationClaim> userOperationClaims = await _userOperationClaimDal.GetListByDynamicAsync(
                request.Dynamic,
                include: c => c.Include(c => c.User).Include(c => c.OperationClaim),
                index: request.PageRequest.Page,
                size: request.PageRequest.PageSize
            );

            // Mapping the user operation claims to a UserOperationClaimListModel object
            UserOperationClaimListModel mappedUserOperationClaimListModel = _mapper.Map<UserOperationClaimListModel>(userOperationClaims);

            return mappedUserOperationClaimListModel;

        }
    }
}
