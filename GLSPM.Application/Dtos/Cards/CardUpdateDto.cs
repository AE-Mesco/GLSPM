using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardUpdateDto
    {
        [Required]
        public int ID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string UserID { get; set; }
    }

    public class CardUpdateDtoValidator : AbstractValidator<CardUpdateDto>
    {
        public CardUpdateDtoValidator()
        {
            RuleFor(c => c.ID)
                .NotEmpty()
                .WithMessage("The card ID is required");

            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("The card title is required")
                .Length(2, 50)
                .WithMessage("The card title should be 2 to 50 charachters");

            RuleFor(c => c.UserID)
                .NotEmpty()
                .WithMessage("The UserID is required");

            When(c => c.Avatar != null, () =>
            {
                RuleFor(c => c.Avatar.Length)
                .ExclusiveBetween(1000, 5000000)
                .WithMessage("The Avatar image size should be 1kb to 5mb");
            });
        }
    }
}
