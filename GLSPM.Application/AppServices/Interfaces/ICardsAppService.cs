using GLSPM.Application.Dtos.Cards;
using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface ICardsAppService : IAppService<Card,int,CardReadDto, CardCreateDto, CardUpdateDto>, ITrasherAppService<CardReadDto, int>
    {
    }
}
