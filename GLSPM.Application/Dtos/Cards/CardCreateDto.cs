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
        public IFormFile? Logo { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string UserID { get; set; }
    }
}
