using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Application.Results.Abstract
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
}
