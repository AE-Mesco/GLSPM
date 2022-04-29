using AutoMapper;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Cards;
using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using CubesFramework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace GLSPM.Application.AppServices
{
    public class CardAppSerivce : AppServiceBase<Card, int, CardReadDto, CardCreateDto, CardUpdateDto>, ICardsAppService
    {
        private readonly Crypto _crypto;
        private readonly string _encryptionCode;
        public CardAppSerivce(IUnitOfWork unitOfWork,
            ILogger<AppServiceBase<Card, int, CardReadDto, CardCreateDto, CardUpdateDto>> logger,
            IRepository<Card, int> repository,
            IMapper mapper,
            HttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            Crypto crypto) : base(unitOfWork, logger, repository, mapper, httpContextAccessor, configuration, environment)
        {
            _crypto = crypto;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }

        public async Task<IEnumerable<CardReadDto>> GetDeletedAsync()
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var data = await dbset.IgnoreQueryFilters()
                                  .Where(c => c.IsSoftDeleted)
                                  .ToArrayAsync();
            return Mapper.Map<IEnumerable<CardReadDto>>(data);
        }

        public async override Task<PagedListDto<CardReadDto>> GetListAsync(GetListDto input)
        {
            IEnumerable<Card> cards;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                input.Filter = input.Filter.ToLower();
                var dbset = await Repository.GetAsQueryableAsync();
                cards = dbset.Where(c => c.Title.ToLower().Contains(input.Filter))
                             .OrderBy(input.Sorting)
                             .Skip(input.SkipCount.Value)
                             .Take(input.MaxResults.Value);
            }
            else
            {
                cards = await Repository.GetAllAsync(input.Sorting, input.SkipCount.Value, input.MaxResults.Value);
            }
            var ressults = Mapper.Map<IReadOnlyList<CardReadDto>>(cards);
            return new PagedListDto<CardReadDto>(cards.Count(), ressults);
        }

        public async Task<bool> IsDeleted(int key)
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var card = await dbset.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ID == key);

            return card != null ? card.IsSoftDeleted : true;
        }

        public async Task MarkAsDeletedAsync(int key)
        {
            if (!await IsDeleted(key))
            {
                var card = await Repository.GetAsync(key);
                card.DeleteDate = DateTime.Now;
                card.IsSoftDeleted = true;
                await UnitOfWork.CommitAsync();
            }
        }

        public async Task<CardReadDto> UnMarkAsDeletedAsync(int key)
        {
            if (await IsDeleted(key))
            {
                var dbset = await Repository.GetAsQueryableAsync();
                var card = await dbset.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.ID == key);
                card.DeleteDate = DateTime.Now;
                card.IsSoftDeleted = false;
                await UnitOfWork.CommitAsync();
                return Mapper.Map<CardReadDto>(card);
            }
            return null;
        }

        public async override Task<CardReadDto> UpdateAsync(int key, CardUpdateDto input)
        {
            var card = await Repository.GetAsync(key);
            if (card != null)
            {
                card.Title = input.Title;
                card.AdditionalInfo = input.AdditionalInfo;
                card.LastUpdateDate = DateTime.Now;
                //card props
                card.HolderName=input.HolderName;
                card.EncriptedCardNumber=await _crypto.EncryptAes(input.CardNumber, _encryptionCode);
                card.EncriptedCVV = await _crypto.EncryptAes(input.CVV, _encryptionCode);
                card.ExpiryMonth=input.ExpiryMonth;
                card.ExpiryYear=input.ExpiryYear;

                await Repository.UpdateAsync(card);
                await UnitOfWork.CommitAsync();
                return Mapper.Map<CardReadDto>(card);
            }
            return null;
        }
    }
}
