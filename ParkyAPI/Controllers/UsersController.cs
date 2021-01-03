




using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/Users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost(Name = "authenticate")]
        public IActionResult Authenticate([FromBody] User userModel)
        {
            var user = _userRepository.Authenticate(userModel.Username, userModel.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Usuário ou senha inválidos!" });
            }

            return Ok(user);
        }
    }
}
