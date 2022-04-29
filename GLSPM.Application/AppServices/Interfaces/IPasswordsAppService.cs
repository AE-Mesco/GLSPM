using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Passwords;
using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface IPasswordsAppService : IAppService<Password,int,PasswordReadDto,PasswordCreateDto,PasswordUpdateDto>, ITrasherAppService<PasswordCreateDto, int>
    {
        Task ChangeLogo(ChangeLogoDto<int> input);
    }
}
