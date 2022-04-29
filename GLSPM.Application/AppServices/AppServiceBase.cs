using AutoMapper;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Domain;
using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices
{
    public class AppServiceBase<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> : IAppService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    {
        public AppServiceBase(IUnitOfWork unitOfWork,
            ILogger<AppServiceBase<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>> logger,
            IRepository<TEntity, TKey> repository,
            IMapper mapper,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IOptions<FilesPathes> filesPathes)
        {
            UnitOfWork = unitOfWork;
            Logger = logger;
            Repository = repository;
            Mapper = mapper;
            Configuration = configuration;
            Environment = environment;
            FilesPathes = filesPathes.Value;
        }
        public IUnitOfWork UnitOfWork { get; }

        public IRepository<TEntity, TKey> Repository { get; }
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public FilesPathes FilesPathes { get; }
        public ILogger Logger { get; }

        public virtual async Task<TReadDto> CreateAsync(TCreateDto input)
        {
            var data = Mapper.Map<TEntity>(input);
            data = await Repository.InsertAsync(data);
            await UnitOfWork.CommitAsync();
            return Mapper.Map<TReadDto>(data);
        }

        public virtual async Task DeleteAsync(TKey key)
        {
            await Repository.DeleteAsync(key);
            await UnitOfWork.CommitAsync();
        }

        public virtual async Task<TReadDto> GetAsync(TKey key)
        {
            var data = await Repository.GetAsync(key);
            return Mapper.Map<TReadDto>(data);
        }

        public virtual async Task<PagedListDto<TReadDto>> GetListAsync(GetListDto input)
        {
            IEnumerable<TEntity> data;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                data = await Repository.GetAllAsync(filter: input.Filter, input.Sorting, skipCound: input.SkipCount.Value, input.MaxResults.Value);
            }
            else
            {
                data = await Repository.GetAllAsync(sorting: input.Sorting, skipCound: input.SkipCount.Value, input.MaxResults.Value);
            }
            var results = Mapper.Map<IReadOnlyList<TReadDto>>(data);
            return new PagedListDto<TReadDto>(data.Count(), results);
        }

        public virtual async Task<TReadDto> UpdateAsync(TKey key, TUpdateDto input)
        {
            if (await Repository.GetAsync(key) != null)
            {
                var data = Mapper.Map<TEntity>(input);
                await Repository.UpdateAsync(data);
                await UnitOfWork.CommitAsync();
                return Mapper.Map<TReadDto>(data);
            }
            return default;
        }
    }
}
