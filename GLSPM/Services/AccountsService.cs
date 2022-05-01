using Blazored.LocalStorage;
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
        private readonly ISyncLocalStorageService _localStorageService;
        const string UserDataKey = "userdata";

        public event Action UserLoginChange;

        public AccountsService(HttpClient httpClient,
            ISyncLocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }
        public bool IsLogged => _localStorageService.ContainKey(UserDataKey);

        public LoginResponseDto User => _localStorageService.GetItem<LoginResponseDto>(UserDataKey);

        public async Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input)
        {
            var response = await _httpClient.PostAsJsonAsync(ApplicationConses.Apis.Accounts.Login, input);
            var loginData = await response.Content.ReadFromJsonAsync<SingleObjectResponse<LoginResponseDto>>();
            if (loginData != null && loginData.Success)
            {
                _localStorageService.SetItem(UserDataKey, loginData.Data);
                UserLoginChange?.Invoke();
            }
            return loginData;
        }

        public async Task Logout()
        {
            _localStorageService.RemoveItem(UserDataKey);
            UserLoginChange?.Invoke();

        }
    }
}
