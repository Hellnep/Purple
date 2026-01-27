using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
using PurpleBackendService.Infrastucture.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Core.DTOs.User;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _customerService;

        public UserController(IUserService service)
        {
            _customerService = service;
        }

        [HttpGet(Name = nameof(GetCustomers))]
        public async Task<ActionResult> GetCustomers()
        {
            var result = await _customerService
                .GetUsersAsync();

            if (result.IsSuccess)
            {
                var users = result.Result as List<UserDTO>;
                var resources = new Resource<List<Resource<UserDTO>>>([]);

                foreach (var user in users!)
                {
                    Resource<UserDTO> resource = new(user);

                    resource.AddLink("patch",
                        Url.Link(nameof(PatchCustomer), new { userId = user.Id })!,
                        HttpMethod.Patch.Method
                    );

                    resources.Data.Add(resource);
                }

                resources.AddLink("self",
                    Url.Link(nameof(GetCustomers), null)!
                );

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{userId}", Name = nameof(GetCustomer))]
        public async Task<ActionResult> GetCustomer(long userId)
        {
            var result = await _customerService
                .GetUserAsync(userId);

            if (result.IsSuccess)
            {
                var resource = new Resource<UserDTO>(result.Result!);

                resource.AddLink("self",
                    Url.Link(nameof(GetCustomer), new { userId })!
                );

                resource.AddLink("patch",
                    Url.Link(nameof(PatchCustomer), new { userId })!,
                    HttpMethod.Patch.Method
                );

                return Ok(resource);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost(Name = nameof(PostCustomer))]
        public async Task<ActionResult> PostCustomer(
            [FromForm] string nickname,
            [FromForm] string? email,
            [FromForm] string? phone
        )
        {
            var customer = Create(nickname, email, phone);

            if (!Validate.TryValidate(customer, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _customerService
                    .CreateUserAsync(customer);

                if (result.IsSuccess)
                {
                    var dataCustomer = result.Result!;
                    var resource = new Resource<UserDTO>(dataCustomer);

                    resource.AddLink("self",
                        Url.Link(nameof(PostCustomer), null)!,
                        HttpMethod.Post.Method
                    );

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        [HttpPatch("{userId}", Name = nameof(PatchCustomer))]
        public async Task<ActionResult> PatchCustomer(long userId,
            [FromForm] string? nickname,
            [FromForm] string? email,
            [FromForm] string? phone
        )
        {
            var user = Create(nickname, email, phone);

            if (!Validate.TryValidate(user, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _customerService
                    .ChangeUserAsync(userId, user);

                if (result.IsSuccess)
                {
                    var resource = new Resource<UserDTO>(result.Result!);

                    resource.AddLink("self",
                        Url.Link(nameof(PatchCustomer), new { userId })!,
                        HttpMethod.Patch.Method
                    );

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        private static UserDTO Create(string? nickname, string? email, string? phone) => new()
        {
            Nickname = nickname,
            Email = email,
            Phone = phone
        };
    }
}
