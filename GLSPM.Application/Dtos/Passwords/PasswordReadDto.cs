﻿using AutoMapper;
using CubesFramework.Security;
using FluentValidation;
using GLSPM.Domain;
using GLSPM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Passwords
{
    public class PasswordReadDto : CriticalEntityBase<int, string>
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Source { get; set; }
    }

    public class PasswordToPasswordReadDtoMappingAction : IMappingAction<Password, PasswordReadDto>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly FilesPathes _filesPathes;
        private readonly string _encryptionCode;

        public PasswordToPasswordReadDtoMappingAction(Crypto crypto,
            IConfiguration configuration,
            IOptions<FilesPathes> filesPathes)
        {
            _crypto = crypto;
            _configuration = configuration;
            _filesPathes = filesPathes.Value;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;

        }
        public void Process(Password source, PasswordReadDto destination, ResolutionContext context)
        {
            destination.Password = _crypto.DecryptAes(source.EncriptedPassword, _encryptionCode).Result;
        }
    }
}