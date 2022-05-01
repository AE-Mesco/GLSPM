using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;

namespace GLSPM.Client.Services.Interfaces
{
    public interface IAccountsService
    {
        Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input);
    }
}
