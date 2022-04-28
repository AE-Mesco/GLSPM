﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string UserID { get; set; }
    }

    public class CardCreateDtoValidator : AbstractValidator<CardCreateDto>
    {
        public CardCreateDtoValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("The Card title is required")
                .Length(2, 50)
                .WithMessage("The card title should be 2 to 50 charachters");

            RuleFor(c => c.UserID)
                .NotEmpty();

            When(c => c.Avatar != null, () =>
            {
                RuleFor(c => c.Avatar.Length)
                .ExclusiveBetween(1000, 5000000)
                .WithMessage("The Avatar image size should be 1kb to 5mb");
            });
        }
    }
}
