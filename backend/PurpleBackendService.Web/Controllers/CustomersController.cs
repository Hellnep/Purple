using Microsoft.AspNetCore.Mvc;
using Purple.Web;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Domain.Entity;

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

        [HttpGet]
        public async Task<ActionResult<List<CustomerDTO>>> Get()
        {
            var result = await _customerService
                .GetCustomersAsync();

            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult> Get(long customerId)
        {
            var result = await _customerService
                .GetCustomerAsync(customerId);

            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost]
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
                    return Ok(result.Result);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }
        
        [HttpPatch]
        public async Task<ActionResult> Patch([FromQuery] long customerId, 
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
                    return Ok(result.Result);
                }
                else
                {
                    return NotFound(result.Errors);
                }          
            }

            return BadRequest();
        }

        private CustomerDTO Create(string? nickname, string? email, string? phone) =>
            new CustomerDTO
            {
                Nickname = nickname,
                Email = email,
                Phone = phone
            };
    }
}