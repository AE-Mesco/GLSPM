using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Passwords;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GLSPM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly ILogger<PasswordsController> _logger;
        private readonly IPasswordsAppService _passwordsAppService;

        public PasswordsController(ILogger<PasswordsController> logger,
            IPasswordsAppService passwordsAppService)
        {
            _logger = logger;
            _passwordsAppService = passwordsAppService;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetListDto input)
        {
            input.Sorting ??= nameof(PasswordReadDto.Title);
            var results= await _passwordsAppService.GetListAsync(input);
            return Ok(results);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var results = await _passwordsAppService.GetAsync(id);
            return results.Success? Ok(results) : BadRequest(results);
        }

        public async Task<IActionResult> Create(PasswordCreateDto input)
        {
            var results=await _passwordsAppService.CreateAsync(input);
            return results.Success ? Ok(results) : BadRequest(results);
        }
    }
}
