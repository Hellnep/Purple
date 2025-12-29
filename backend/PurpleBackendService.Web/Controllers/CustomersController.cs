using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Web.Resource;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService service)
        {
            _customerService = service;
        }

        [HttpGet(Name = nameof(Get))]
        public ActionResult Get()
        {
            var result = _customerService
                .GetCustomers();

            if (result.IsSuccess)
            {
                var resource = new Resource<ICollection<CustomerDTO>>(result.Result!);
                resource.AddLink("self", Url.Link(nameof(Get), null)!);

                return Ok(resource);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{customerId}", Name = nameof(GetCustomer))]
        public ActionResult GetCustomer(long customerId)
        {
            var result = _customerService
                .GetCustomer(customerId);

            if (result.IsSuccess)
            {
                var resource = new Resource<CustomerDTO>(result.Result!);

                resource.AddLink("self", Url.Link(nameof(GetCustomer), new { customerId })!);
                resource.AddLink("change", Url.Link(nameof(Patch), new { customerId })!, HttpMethod.Patch.Method);

                return Ok(resource);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost(Name = nameof(Post))]
        public async Task<ActionResult> Post(
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
                    .CreateCustomerAsync(customer);

                if (result.IsSuccess)
                {
                    var dataCustomer = result.Result!;
                    var resource = new Resource<CustomerDTO>(dataCustomer);

                    resource.AddLink("self", Url.Link(nameof(Post), null)!, HttpMethod.Post.Method);
                    resource.AddLink("get", Url.Link(nameof(GetCustomer), new { customerId = dataCustomer.Id })!);
                    resource.AddLink("change", Url.Link(nameof(Patch), new { customerId = dataCustomer.Id })!, HttpMethod.Patch.Method);

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        [HttpPatch("{customerId}", Name = nameof(Patch))]
        public async Task<ActionResult> Patch(long customerId,
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
                    .ChangeCustomerAsync(customerId, customer);

                if (result.IsSuccess)
                {
                    var resource = new Resource<CustomerDTO>(result.Result!);
                    resource.AddLink("self", Url.Link(nameof(GetCustomer), new { customerId })!);

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        private static CustomerDTO Create(string? nickname, string? email, string? phone) =>
            new()
            {
                Nickname = nickname,
                Email = email,
                Phone = phone
            };
    }
}