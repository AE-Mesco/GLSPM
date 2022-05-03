using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using GLSPM.Domain.Dtos.Identity;

namespace GLSPM.Client.Services
{
    public class GLSPMAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public GLSPMAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            _jwtSecurityTokenHandler = new();
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var user = await _localStorageService.GetItemAsync<LoginResponseDto>(LocalStorageUserDataKey);
                if (user == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var jwttoken = _jwtSecurityTokenHandler.ReadJwtToken(user.Token);
                if (jwttoken.ValidTo<DateTime.UtcNow)
                {
                    await _localStorageService.RemoveItemAsync(LocalStorageUserDataKey);
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = await ParseUserClaims(jwttoken);
                var authUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(authUser);
            }
            catch 
            {

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        private async Task<IList<Claim>> ParseUserClaims(JwtSecurityToken jwttoken)
        {
            var claims = jwttoken.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, jwttoken.Subject));
            return claims;
        }

        public async Task SingIn()
        {
            var user = await _localStorageService.GetItemAsync<LoginResponseDto>(LocalStorageUserDataKey);
            var jwttoken = _jwtSecurityTokenHandler.ReadJwtToken(user.Token);
            var claims = await ParseUserClaims(jwttoken);
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            Task<AuthenticationState> authenticate = Task.FromResult(new AuthenticationState(authUser));
            NotifyAuthenticationStateChanged(authenticate);
        }

        public async Task SignOut()
        {
            var guest = new ClaimsPrincipal(new ClaimsIdentity());
            Task<AuthenticationState> authenticate = Task.FromResult(new AuthenticationState(guest));
            NotifyAuthenticationStateChanged(authenticate);

        }
    }
}
