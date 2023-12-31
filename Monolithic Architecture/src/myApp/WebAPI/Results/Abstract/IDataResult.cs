﻿namespace WebAPI.Results.Abstract;

public interface IDataResult<T> : IResult
{
    T Data { get; }
}
