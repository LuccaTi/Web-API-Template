using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPITemplate.Application.DTOs.Requests;

namespace WebAPITemplate.Application.Validations
{
    public class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequestDto>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(request => request.Id)
                .NotEmpty().WithMessage("Id is mandatory.")
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
        }
    }
}
