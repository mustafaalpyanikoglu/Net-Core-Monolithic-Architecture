﻿using WebAPI.Contexts;
using WebAPI.Models.Concrete;
using WebAPI.Persistence.Repositories;
using WebAPI.Repositories.Abstract;

namespace WebAPI.Repositories.Concrete;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context)
        : base(context) { }
}
