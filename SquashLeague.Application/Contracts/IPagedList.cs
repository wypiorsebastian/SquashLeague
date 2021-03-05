using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Application.Contracts
{
    public interface IPagedList
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int PageSize { get; }
        int TotalCount { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
        int PageNumber { get; }
    }
}
