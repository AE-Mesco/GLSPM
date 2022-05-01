using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Passwords
{
    public class PasswordUpdateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Source { get; set; }
    }

    public class PasswordUpdateDtoValidator:AbstractValidator<PasswordUpdateDto>
    {
        public PasswordUpdateDtoValidator()
        {
            RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage("The title is required")
            .Length(2, 50)
            .WithMessage("The title should be 2 to 50 charachters");

            RuleFor(p => p.Password)
                .NotEmpty()
                .WithMessage("The password is required");

            RuleFor(p => p.Username)
               .NotEmpty()
               .WithMessage("The username is required");
        }
    }
}
