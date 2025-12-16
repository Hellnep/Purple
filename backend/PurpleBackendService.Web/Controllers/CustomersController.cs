using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private ICustomerService _customerService;

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
        public async Task<ActionResult> Post([FromBody] CustomerDTO customer)
        {
            if (!Validate.TryValidate(customer, out var results))
            {
                return BadRequest(results);
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
        }
        
        [HttpPatch]
        public async Task<ActionResult> Patch([FromQuery] long customerId, 
            [FromBody] CustomerDTO customer
        )
        {
            if (!Validate.TryValidate(customer, out var results))
            {
                return BadRequest(results);
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
        }
    }
}