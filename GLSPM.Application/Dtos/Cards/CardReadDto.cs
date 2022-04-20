using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardReadDto : CriticalEntityBase<int, string>
    {
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string HolderName { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public int ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string CVV { get; set; }
    }
}
