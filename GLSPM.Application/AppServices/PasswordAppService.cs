﻿using AutoMapper;
using CubesFramework.Security;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Passwords;
using GLSPM.Domain;
using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Passwords;
using GLSPM.Application.Helpers;

namespace GLSPM.Application.AppServices
{
    public class PasswordAppService : AppServiceBase<Password, int, PasswordReadDto, PasswordCreateDto, PasswordUpdateDto>, IPasswordsAppService
    {
        private readonly Crypto _crypto;
        private readonly string _encryptionCode;

        public PasswordAppService(IUnitOfWork unitOfWork,
            ILogger<AppServiceBase<Password, int, PasswordReadDto, PasswordCreateDto, PasswordUpdateDto>> logger,
            IRepository<Password, int> repository,
            IMapper mapper,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IOptions<FilesPathes> filesPathes,
            IUriAppService uriAppService,
            IHttpContextAccessor httpContextAccessor,
            Crypto crypto) : base(unitOfWork, logger, repository, mapper, configuration, environment, filesPathes, uriAppService, httpContextAccessor)
        {
            _crypto = crypto;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }

        public async Task ChangeLogo(ChangeLogoDto<int> input)
        {
            var password = await Repository.GetAsync(input.Key);
            if (password != null)
            {
                if (!string.IsNullOrEmpty(password.LogoPath) && File.Exists(password.LogoPath))
                {
                    File.Delete(password.LogoPath);
                }
                var logoPath = Path.Combine(Path.GetFullPath(FilesPathes.LogosPath), $"{DateTime.Now.ToFileTime()}{Path.GetExtension(input.Logo.FileName)}");
                using (FileStream logoStream = new FileStream(logoPath, FileMode.Create))
                {
                    await input.Logo.CopyToAsync(logoStream);
                    await logoStream.FlushAsync();
                    password.LogoPath = logoPath;
                    await Repository.UpdateAsync(password);
                    await UnitOfWork.CommitAsync();
                }
            }
        }
        public async override Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetListAsync(GetListDto input)
        {
            IEnumerable<Password> passwords;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                input.Filter = input.Filter.ToLower();
                var dbset = await Repository.GetAsQueryableAsync();
                passwords = dbset.Where(c => c.Title.ToLower().Contains(input.Filter) ||
                c.Source.ToLower().Contains(input.Filter))
                             .OrderBy(input.Sorting)
                             .Skip(input.SkippedData)
                             .Take(input.PageSize);
            }
            else
            {
                passwords = await Repository.GetAllAsync(input.Sorting, input.SkippedData, input.PageSize);
            }
            var getAllQuery = await Repository.GetAllAsync(filter: input.Filter, input.Sorting, skipCound: 0, int.MaxValue);
            var results = Mapper.Map<IEnumerable<PasswordReadDto>>(passwords);
            var response = PaginationHelper.CreatePagedReponse(results, input, getAllQuery.Count(), UriAppService, HttpContextAccessor.HttpContext.Request.Path.Value);
            response.Success = true;
            response.Message = "Items Found";
            response.StatusCode = StatusCodes.Status200OK;
            return response;
        }

        public async override Task<SingleObjectResponse<PasswordReadDto>> UpdateAsync(int key, PasswordUpdateDto input)
        {
            var password = await Repository.GetAsync(key);
            if (password != null)
            {
                password.Title = input.Title;
                password.AdditionalInfo = input.AdditionalInfo;
                password.LastUpdateDate = DateTime.Now;
                password.Source = input.Source;
                password.EncriptedPassword = await _crypto.EncryptAes(input.Password, _encryptionCode);
                password.Username = input.Username;

                await Repository.UpdateAsync(password);
                await UnitOfWork.CommitAsync();
                var results = Mapper.Map<PasswordReadDto>(password);
                return new SingleObjectResponse<PasswordReadDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status202Accepted,
                    Message = "Item Updated",
                    Data = results
                };
            }
            return new SingleObjectResponse<PasswordReadDto>
            {
                Success = false,
                StatusCode = StatusCodes.Status200OK,
                Message = "Item Not Found",
                Error = "Couldn't find an item realted to the passed id"
            };
        }
        public async Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetDeletedAsync(PaginationParametersBase pagination)
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var data = await dbset.IgnoreQueryFilters()
                                  .Where(c => c.IsSoftDeleted)
                                  .ToArrayAsync();
            var results = Mapper.Map<IEnumerable<PasswordReadDto>>(data);
            var response = PaginationHelper.CreatePagedReponse(results, pagination, data.Count(), UriAppService, HttpContextAccessor.HttpContext.Request.Path.Value);
            response.Success = true;
            response.Message = "Items Found";
            response.StatusCode = StatusCodes.Status200OK;
            return response;
        }

        public async Task<bool> IsDeleted(int key)
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var password = await dbset.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ID == key);

            return password != null ? password.IsSoftDeleted : false;
        }

        public async Task MarkAsDeletedAsync(int key)
        {
            if (!await IsDeleted(key))
            {
                var password = await Repository.GetAsync(key);
                password.DeleteDate = DateTime.Now;
                password.IsSoftDeleted = true;
                await UnitOfWork.CommitAsync();
            }
        }

        public async Task<SingleObjectResponse<PasswordReadDto>> UnMarkAsDeletedAsync(int key)
        {
            if (await IsDeleted(key))
            {
                var dbset = await Repository.GetAsQueryableAsync();
                var password = await dbset.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.ID == key);
                password.DeleteDate = DateTime.Now;
                password.IsSoftDeleted = false;
                await UnitOfWork.CommitAsync();
                var results = Mapper.Map<PasswordReadDto>(password);
                return new SingleObjectResponse<PasswordReadDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status202Accepted,
                    Message = "Item Restored",
                    Data = results
                };
            }
            return new SingleObjectResponse<PasswordReadDto>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Item Not Found",
                Error = "Couldn't find an item realted to the passed id"
            };
        }
    }
}
