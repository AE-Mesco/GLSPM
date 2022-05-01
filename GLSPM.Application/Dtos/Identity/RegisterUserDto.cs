using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Identity
{
    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public IFormFile? Avatar { get; set; }
    }

    public class RegisterUserDtoValidator:AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(u=>u.Username)
                .NotEmpty()
                .MinimumLength(4);

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();

            When(u => u.Avatar != null, () =>
            {
                RuleFor(u => u.Avatar.Length)
                .ExclusiveBetween(1000, 5000000);
            });
        }
    }
}
