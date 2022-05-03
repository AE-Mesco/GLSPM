using GLSPM.Client.Services.Interfaces;
using GLSPM.Domain.Dtos;
using Microsoft.AspNetCore.Components.Forms;

namespace GLSPM.Client.Services
{
    public class PasswordsService : IPasswordsService
    {
        private readonly HttpClient _httpClient;

        public PasswordsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public event Action PasswordsChnaged;

        public Task ChangeLogoAsync(int id, IBrowserFile logo)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordReadDto> CreateAsync(PasswordCreateDto input, IBrowserFile logo)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PasswordReadDto> GetAsync(int id)
        {
            var results = await _httpClient.GetAsync<>(Passwords.GetOne(id));

        }

        public Task<IEnumerable<PasswordReadDto>> GetListAsync(GetListDto input)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PasswordReadDto>> GetTrashedListAsync(GetListDto input)
        {
            throw new NotImplementedException();
        }

        public Task MoveToTrashAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordReadDto> RestoreAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordReadDto> UpdateAsync(PasswordUpdateDto input)
        {
            throw new NotImplementedException();
        }
    }
}
