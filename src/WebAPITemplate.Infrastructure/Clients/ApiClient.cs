using Microsoft.Extensions.Logging;
using WebAPITemplate.Application.Interfaces.Clients;
using WebAPITemplate.Application.Models;
using WebAPITemplate.Infrastructure.Clients.Internal;

namespace WebAPITemplate.Infrastructure.Clients
{
    public class ApiClient : ApiClientBase, IApiClient
    {
        private const string _className = "ApiClient";
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger) : base(httpClient, logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await base.GetAllAsync<UserModel>("users", cancellationToken);
        }

        public async Task<UserModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await base.GetByIdAsync<UserModel>($"users/{id}", cancellationToken);
        }
    }
}
