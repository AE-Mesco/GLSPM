using AutoMapper;
using GLSPM.Application.Dtos;
using GLSPM.Domain;
using GLSPM.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface IAppService<TEntity,TKey,TReadDto,TCreateDto,TUpdateDto>
    {
        IUnitOfWork UnitOfWork { get; }
        IRepository<TEntity,TKey> Repository { get; }
        ILogger Logger { get; }
        IMapper Mapper { get; }
        IConfiguration Configuration { get; }
        IWebHostEnvironment Environment { get; }
        FilesPathes FilesPathes { get; }
        Task<TReadDto> GetAsync(TKey key);
        Task<PagedListDto<TReadDto>> GetListAsync(GetListDto input);
        Task<TReadDto> UpdateAsync(TKey key, TUpdateDto input);
        Task<TReadDto> CreateAsync(TCreateDto input);
        Task DeleteAsync(TKey key);
    }
}
