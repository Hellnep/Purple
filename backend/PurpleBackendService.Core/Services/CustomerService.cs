using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<CustomerDTO>> CreateCustomerAsync(CustomerDTO input)
        {
            Customer customer = Mapping
                .Get<Customer, CustomerDTO>(input);

            if (customer.Email is null)
                return OperationResult<CustomerDTO>.Failure("You need to enter an email address");

            if (_repository.EmailExists(customer.Email))
                return OperationResult<CustomerDTO>.Failure("A user with such an email address already exists");

            var result = await _repository.Add(customer);
            
            return OperationResult<CustomerDTO>
                .Success(Mapping.Get<CustomerDTO, Customer>(result));
        }

        public async Task<OperationResult<CustomerDTO>> GetCustomerAsync(long id)
        {
            var result = _repository.Get(id);
            
            return OperationResult<CustomerDTO>
                .Success(Mapping.Get<CustomerDTO, Customer>(result));
        }

        public async Task<OperationResult<ICollection<CustomerDTO>>> GetCustomersAsync()
        {
            var customers = _repository.Get();
            var result = new List<CustomerDTO>();

            foreach (Customer customer in customers)
                result.Add(Mapping.Get<CustomerDTO, Customer>(customer));

            return OperationResult<ICollection<CustomerDTO>>
                .Success(result);
        }

        public async Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input)
        {
            try
            {
                var newData = Mapping.Get<Customer, CustomerDTO>(input);
                var customer = _repository.Get(id);

                if (newData.FirstName is not null)
                    customer.FirstName = newData.FirstName;

                if (newData.Email is not null)
                    customer.Email = newData.Email;

                if (newData.Phone is not null)
                    customer.Phone = newData.Phone;

                await _repository.Update();
                
                return OperationResult<CustomerDTO>
                    .Success(Mapping.Get<CustomerDTO, Customer>(customer));
            }
            catch (ArgumentNullException exception)
            {
                return OperationResult<CustomerDTO>.Failure(exception.Message);
            }
        }
    }
}
