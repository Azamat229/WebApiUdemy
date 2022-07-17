using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto.User;
using WebApi.Dto.Character.User;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]// tag
    [Route("[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {

            var response = await _authRepo.Register(
                new User { Username = request.Username }, request.Password); // не понятная реализация 
            // нужно изучить сокращения

            if (!response.Success)
            { 
                return BadRequest(response); 
            }
            return Ok(response);    

        }


        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {

            var response = await _authRepo.Login(request.Username, request.Password);
                // не понятная реализация 
            // нужно изучить сокращения

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }

    }
}
