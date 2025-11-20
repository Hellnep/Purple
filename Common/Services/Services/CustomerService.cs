using System.ComponentModel.DataAnnotations;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Mapping;
using Purple.Common.Database.Repository;
using Purple.Common.ModelValidator;
using Purple.Common.Services.Interface;

namespace Purple.Common.Services;

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
            OperationResult<CustomerDTO>.Failure("You need to enter an email address");

#pragma warning disable CS8604
        if (_repository.EmailExists(customer.Email))
            OperationResult<CustomerDTO>.Failure("A user with such an email address already exists");

        var result = _repository.Add(customer).Result;
        
        return OperationResult<CustomerDTO>
            .Success(Mapping.Get<CustomerDTO, Customer>(result));
    }

    public async Task<OperationResult<CustomerDTO>> GetCustomerAsync(long id)
    {
        var result = _repository.Get(id);
        
        return OperationResult<CustomerDTO>
            .Success(Mapping.Get<CustomerDTO, Customer>(result));
    }

    public async Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input)
    {
        try
        {
            var customer = _repository.Get(id);
            var newData = Mapping.Get<Customer, CustomerDTO>(input);

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
