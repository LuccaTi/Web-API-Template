using WebAPITemplate.Application.Models;

namespace WebAPITemplate.Application.Interfaces.Clients;

public interface IApiClient
{
    public Task<IEnumerable<UserModel>> GetAllAsync();
    public Task<UserModel?> GetByIdAsync(long id);
}
