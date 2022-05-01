using GLSPM.Client.Services.Interfaces;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using System.Net.Http.Json;

namespace GLSPM.Client.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly HttpClient _httpClient;

        public AccountsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input)
        {
            var results = await _httpClient.PostAsJsonAsync(ApplicationConses.Apis.Accounts.Login, input);
            return await results.Content.ReadFromJsonAsync<SingleObjectResponse<LoginResponseDto>>();
        }
    }
}
