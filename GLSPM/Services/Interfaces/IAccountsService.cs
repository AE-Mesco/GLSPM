using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;

namespace GLSPM.Client.Services.Interfaces
{
    public interface IAccountsService
    {
        event Action UserLoginChange;
        Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input);
        Task Logout();
        bool IsLogged { get;}
        LoginResponseDto User { get;}

    }
}
