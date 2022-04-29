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
            Crypto crypto) : base(unitOfWork, logger, repository, mapper, configuration, environment, filesPathes)
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
        public async override Task<PagedListDto<PasswordReadDto>> GetListAsync(GetListDto input)
        {
            IEnumerable<Password> passwords;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                input.Filter = input.Filter.ToLower();
                var dbset = await Repository.GetAsQueryableAsync();
                passwords = dbset.Where(c => c.Title.ToLower().Contains(input.Filter))
                             .OrderBy(input.Sorting)
                             .Skip(input.SkipCount.Value)
                             .Take(input.MaxResults.Value);
            }
            else
            {
                passwords = await Repository.GetAllAsync(input.Sorting, input.SkipCount.Value, input.MaxResults.Value);
            }
            var ressults = Mapper.Map<IReadOnlyList<PasswordReadDto>>(passwords);
            return new PagedListDto<PasswordReadDto>(passwords.Count(), ressults);
        }
        public async override Task<PasswordReadDto> UpdateAsync(int key, PasswordUpdateDto input)
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
                return Mapper.Map<PasswordReadDto>(password);
            }
            return null;
        }
        public async Task<IEnumerable<PasswordCreateDto>> GetDeletedAsync()
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var data = await dbset.IgnoreQueryFilters()
                                  .Where(c => c.IsSoftDeleted)
                                  .ToArrayAsync();
            return Mapper.Map<IEnumerable<PasswordCreateDto>>(data);
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

        public async Task<PasswordCreateDto> UnMarkAsDeletedAsync(int key)
        {
            if (await IsDeleted(key))
            {
                var dbset = await Repository.GetAsQueryableAsync();
                var card = await dbset.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.ID == key);
                card.DeleteDate = DateTime.Now;
                card.IsSoftDeleted = false;
                await UnitOfWork.CommitAsync();
                return Mapper.Map<PasswordCreateDto>(card);
            }
            return null;
        }
    }
}
