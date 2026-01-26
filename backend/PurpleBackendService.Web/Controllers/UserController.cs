using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;

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
                var customers = result.Result as List<UserDTO>;
                List<Resource<UserDTO>> resources = [];

                foreach (var customer in customers!)
                {
                    Resource<UserDTO> resource = new(customer);

                    resource.AddLink("get", Url.Link(nameof(GetCustomer), new { customerId = customer.Id})!);
                    resource.AddLink("patch",
                        Url.Link(nameof(PatchCustomer), new
                        {
                            customerId = customer.Id
                        })!,
                        HttpMethod.Patch.Method
                    );

                    resources.Add(resource);
                }

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{customerId}", Name = nameof(GetCustomer))]
        public async Task<ActionResult> GetCustomer(long customerId)
        {
            var result = await _customerService
                .GetUserAsync(customerId);

            if (result.IsSuccess)
            {
                var resource = new Resource<UserDTO>(result.Result!);

                resource.AddLink("self", Url.Link(nameof(GetCustomer), new { customerId })!);
                resource.AddLink("patch", Url.Link(nameof(PatchCustomer),
                    new { customerId })!,
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

                    resource.AddLink("self", Url.Link(nameof(PostCustomer), null)!, HttpMethod.Post.Method);
                    resource.AddLink("get", Url.Link(nameof(GetCustomer), new { customerId = dataCustomer.Id })!);
                    resource.AddLink("patch", Url.Link(nameof(PatchCustomer), new { customerId = dataCustomer.Id })!, HttpMethod.Patch.Method);

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        [HttpPatch("{customerId}", Name = nameof(PatchCustomer))]
        public async Task<ActionResult> PatchCustomer(long customerId,
            [FromForm] string? nickname,
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
                    .ChangeUserAsync(customerId, customer);

                if (result.IsSuccess)
                {
                    var resource = new Resource<UserDTO>(result.Result!);

                    resource.AddLink("self", Url.Link(nameof(PatchCustomer), new { customerId })!, HttpMethod.Patch.Method);
                    resource.AddLink("get", Url.Link(nameof(GetCustomer), new { customerId })!);

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
