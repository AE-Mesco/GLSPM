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
        [HttpGet("Trashed")]
        public async Task<IActionResult> GetTrashed()
        {
            var results = await _passwordsAppService.GetDeletedAsync();
            return Ok(results);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PasswordCreateDto input)
        {
            var results=await _passwordsAppService.CreateAsync(input);
            return results.Success ? Created("",results) : BadRequest(results);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody]PasswordUpdateDto input)
        {
            var results = await _passwordsAppService.UpdateAsync(id,input);
            return results.Success ? Accepted(results) : BadRequest(results);
        }
        [HttpPut("MoveToTrash/{id}")]
        public async Task<IActionResult> MoveToTrash(int id)
        {
            await _passwordsAppService.MarkAsDeletedAsync(id);
            return Accepted(new SingleObjectResponse<object>
            {
                Success = true,
                StatusCode=StatusCodes.Status202Accepted,
                Message="Item Moved to Trash"
            });
        }
        [HttpPut("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var results=await _passwordsAppService.UnMarkAsDeletedAsync(id);
            return results.Success ? Accepted(results) : BadRequest(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _passwordsAppService.DeleteAsync(id);
            return NoContent();
        }
    }
}
