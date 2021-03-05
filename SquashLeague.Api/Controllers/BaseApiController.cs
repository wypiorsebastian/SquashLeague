using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SquashLeague.Application.Contracts;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SquashLeague.Api.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        private IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(request, cancellationToken);

            if (result is IPagedList pagedListResult)
            {
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(new
                {
                    totalCount = pagedListResult.TotalCount,
                    pageSize = pagedListResult.PageSize,
                    currentPage = pagedListResult.CurrentPage,
                    totalPages = pagedListResult.TotalPages
                }));
            }

            return result;
        }

    }
}
