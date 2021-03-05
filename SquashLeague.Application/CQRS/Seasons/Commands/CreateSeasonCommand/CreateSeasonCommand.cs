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

namespace SquashLeague.Application.CQRS.Seasons.Commands.CreateSeasonCommand
{
    public class CreateSeasonCommand : IRequest<SeasonDTO>
    {
        internal SeasonCreateDTO SeasonCreateDTO { get; private set; }

        public CreateSeasonCommand(SeasonCreateDTO seasonCreateDTO)
        {
            SeasonCreateDTO = seasonCreateDTO ?? throw new ArgumentNullException(nameof(seasonCreateDTO));
        }
    }

    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, SeasonDTO>
    {
        private readonly IAsyncRepository<Season> _repository;
        private readonly IMapper _mapper;
        public async Task<SeasonDTO> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            var entityToCreate = _mapper.Map<Season>(request.SeasonCreateDTO);

            var entity = await _repository.AddAsync(entityToCreate);

            return (_mapper.Map<SeasonDTO>(entity));
        }
    }
}
