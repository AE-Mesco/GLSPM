using Blazored.LocalStorage;
using GLSPM.Client.Services.Interfaces;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GLSPM.Client.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncLocalStorageService _localStorageService;
        private readonly GLSPMAuthenticationStateProvider _authenticationStateProvider;

        public event Action UserLoginChange;

        public AccountsService(HttpClient httpClient,
            ISyncLocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider =(GLSPMAuthenticationStateProvider) authenticationStateProvider;
        }
        public bool IsLogged => _localStorageService.ContainKey(LocalStorageUserDataKey);

        public LoginResponseDto User => _localStorageService.GetItem<LoginResponseDto>(LocalStorageUserDataKey);

        public async Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input)
        {
            var response = await _httpClient.PostAsJsonAsync(ApplicationConses.Apis.Accounts.Login, input);
            var loginData = await response.Content.ReadFromJsonAsync<SingleObjectResponse<LoginResponseDto>>();
            if (loginData != null && loginData.Success)
            {
                _localStorageService.SetItem(LocalStorageUserDataKey, loginData.Data);
                await _authenticationStateProvider.SingIn();
                UserLoginChange?.Invoke();
            }
            return loginData;
        }

        public async Task Logout()
        {
            _localStorageService.RemoveItem(LocalStorageUserDataKey);
            await _authenticationStateProvider.SignOut();
            UserLoginChange?.Invoke();

        }

        public async Task<SingleObjectResponse<object>> Register(RegisterUserDto input)
        {
            throw new NotImplementedException();
        }

    }
}
