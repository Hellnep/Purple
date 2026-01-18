using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;

namespace PurpleBackendService.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<UserDTO>> CreateCustomerAsync(UserDTO input)
        {
            User customer = Mapping
                .Get<User, UserDTO>(input);

            if (customer.Email is null)
            {
                return OperationResult<UserDTO>.Failure("You need to enter an email address");
            }

            if (await _repository.EmailExists(customer.Email))
            {
                return OperationResult<UserDTO>.Failure("A user with such an email address already exists");
            }

            var result = await _repository.Add(customer);

            return OperationResult<UserDTO>
                .Success(Mapping.Get<UserDTO, User>(result));
        }

        public async Task<OperationResult<UserDTO>> GetCustomerAsync(long customerId)
        {
            var customer = await _repository.Get(customerId);

            if (customer is null)
            {
                return OperationResult<UserDTO>
                    .Failure($"Customer with ID={customerId} not found");
            }

            return OperationResult<UserDTO>
                .Success(Mapping.Get<UserDTO, User>(customer));
        }

        public async Task<OperationResult<ICollection<UserDTO>>> GetCustomersAsync()
        {
            var customers = await _repository.Get();
            var result = new List<UserDTO>();

            foreach (User customer in customers)
            {
                result.Add(Mapping.Get<UserDTO, User>(customer));
            }

            return OperationResult<ICollection<UserDTO>>
                .Success(result);
        }

        public async Task<OperationResult<UserDTO>> ChangeCustomerAsync(long customerId, UserDTO input)
        {
            try
            {
                var newData = Mapping.Get<User, UserDTO>(input);
                var customer = await _repository.Get(customerId);

                if (customer is null)
                {
                    return OperationResult<UserDTO>.Failure("Customer not found");
                }

                if (newData.Nickname is not null)
                {
                    customer.Nickname = newData.Nickname;
                }

                if (newData.Email is not null)
                {
                    customer.Email = newData.Email;
                }

                if (newData.Phone is not null)
                {
                    customer.Phone = newData.Phone;
                }

                await _repository.Update();

                return OperationResult<UserDTO>
                    .Success(Mapping.Get<UserDTO, User>(customer));
            }
            catch (ArgumentNullException exception)
            {
                return OperationResult<UserDTO>.Failure(exception.Message);
            }
        }
    }
}
