using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos
{
    public class ChangeLogoDto
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public IFormFile Logo { get; set; }
    }

    public class ChangeLogoDtoValidator:AbstractValidator<ChangeLogoDto>
    {
        public ChangeLogoDtoValidator()
        {
            RuleFor(l => l.Key)
                   .NotEmpty();
            RuleFor(l => l.Logo)
                    .NotEmpty();
            RuleFor(l => l.Logo.Length)
                    .ExclusiveBetween(1000, 5000000);
        }
    }
}
