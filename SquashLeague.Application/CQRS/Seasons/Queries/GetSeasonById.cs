using AutoMapper;
using MediatR;
using SquashLeague.Application.Contracts.Persistence;
using SquashLeague.Application.CQRS.Seasons.DTOs;
using SquashLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquashLeague.Application.CQRS.Seasons.Queries
{
    public class GetSeasonById : IRequest<SeasonDTO>
    {
        internal int SeasonId { get; private set; }

        public GetSeasonById(int seasonId)
        {
            SeasonId = seasonId;
        }
    }

    public class GetSeasonByIdHandler : IRequestHandler<GetSeasonById, SeasonDTO>
    {
        private readonly IAsyncRepository<Season> _repository;
        private readonly IMapper _mapper;

        public async Task<SeasonDTO> Handle(GetSeasonById request, CancellationToken cancellationToken)
        {
            var season = await _repository.GetByIdAsync(request.SeasonId);

            return _mapper.Map<SeasonDTO>(season);
        }
    }
}
