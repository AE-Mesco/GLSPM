using AutoMapper;
using GLSPM.Application.Dtos.Cards;
using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos
{
    public class GLSPMMappingProfile : Profile
    {
        public GLSPMMappingProfile()
        {
            RegisterCards();
        }

        private void RegisterCards()
        {
            CreateMap<CardCreateDto, Card>();
            CreateMap<Card, CardReadDto>()
                .BeforeMap<CardToCardReadDtoMappingAction>()
                .ReverseMap()
                .BeforeMap<CardReadDtoToCardMappingAction>();
        }
    }
}
