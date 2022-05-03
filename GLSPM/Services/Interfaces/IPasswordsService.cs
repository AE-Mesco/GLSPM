﻿using GLSPM.Domain.Dtos;
using Microsoft.AspNetCore.Components.Forms;

namespace GLSPM.Client.Services.Interfaces
{
    public interface IPasswordsService
    {
        event Action PasswordsChnaged;
        Task<SingleObjectResponse< PasswordReadDto>> GetAsync(int id);
        Task<MultiObjectsResponse<PasswordReadDto>> GetListAsync(GetListDto input);
        Task<SingleObjectResponse<PasswordReadDto>> CreateAsync(PasswordCreateDto input,IBrowserFile logo);
        Task<SingleObjectResponse<PasswordReadDto>> UpdateAsync(PasswordUpdateDto input);
        Task ChangeLogoAsync(int id,IBrowserFile logo);
        Task DeleteAsync(int id);
        Task MoveToTrashAsync(int id);
        Task<SingleObjectResponse<PasswordReadDto>> RestoreAsync(int id);
        Task<IEnumerable<PasswordReadDto>> GetTrashedListAsync(GetListDto input);
    }
}