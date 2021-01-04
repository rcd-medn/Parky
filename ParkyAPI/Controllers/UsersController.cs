




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
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthenticationModel userModel)
        {
            var user = _userRepository.Authenticate(userModel.Username, userModel.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Usuário ou senha inválidos!" });
            }

            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] AuthenticationModel userModel)
        {
            bool ifUserNameUnique = _userRepository.IsUniqueUser(userModel.Username);
            if (!ifUserNameUnique)
            {
                return BadRequest(new { message = "O nome de usuário já existe!" });
            }

            var user = _userRepository.Register(userModel.Username, userModel.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Ocorreu um erro ao registrar o novo usuário!" });
            }

            return Ok();
        }
    }
}
