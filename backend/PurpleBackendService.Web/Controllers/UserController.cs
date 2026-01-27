using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
using PurpleBackendService.Web.Models.DTOs.User;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Domain.Entity;

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
                var users = new List<UserDTO>();

                foreach (var user in result.Result!)
                {
                    users.Add(Mapping.Get<UserDTO, User>(user));
                }

                var resources = new ResourceCollection<ResourceObject<UserDTO>>([]);

                foreach (var user in users!)
                {
                    ResourceObject<UserDTO> resource = new(user);

                    resource.AddLink("patch",
                        Url.Link(nameof(PatchCustomer), new { userId = user.Id })!,
                        HttpMethod.Patch.Method
                    );

                    resources.Items.Add(resource);
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
                var user = Mapping.Get<UserDTO, User>(result.Result!);
                var resource = new ResourceObject<UserDTO>(user);

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
            var createdUser = Create(nickname, email, phone);

            if (!Validate.TryValidate(createdUser, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _customerService
                    .CreateUserAsync(Mapping.Get<User, UserDTO>(createdUser));

                if (result.IsSuccess)
                {
                    var user = Mapping.Get<UserDTO, User>(result.Result!);
                    var resource = new ResourceObject<UserDTO>(user);

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
            var createdUser = Create(nickname, email, phone);

            if (!Validate.TryValidate(createdUser, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _customerService
                    .ChangeUserAsync(userId,
                        Mapping.Get<User, UserDTO>(createdUser)
                    );

                if (result.IsSuccess)
                {
                    var user = Mapping.Get<UserDTO, User>(result.Result!);
                    var resource = new ResourceObject<UserDTO>(user);

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
