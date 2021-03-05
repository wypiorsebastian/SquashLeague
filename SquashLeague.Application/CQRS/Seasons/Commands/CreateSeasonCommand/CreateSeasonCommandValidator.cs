using FluentValidation;
using SquashLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Application.CQRS.Seasons.Commands.CreateSeasonCommand
{
    public class CreateSeasonCommandValidator : AbstractValidator<Season>
    {
        public CreateSeasonCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Pole opisu sezonu nie może puste")
                .NotNull();
        }
    }
}
