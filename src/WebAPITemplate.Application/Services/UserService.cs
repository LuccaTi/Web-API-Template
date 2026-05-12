using WebAPITemplate.Application.Interfaces.Services;
using System.Diagnostics.Metrics;
using WebAPITemplate.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using WebAPITemplate.Application.DTOs.Responses;
using Mapster;
using WebAPITemplate.Domain.Exceptions;
using FluentValidation;
using WebAPITemplate.Application.DTOs.Requests;
using WebAPITemplate.Application.Extensions;

namespace WebAPITemplate.Application.Services
{
    public class UserService : IUserService
    {
        private const string className = "UserService";
        private readonly ILogger<UserService> _logger;
        private readonly IApiClient _apiClient;
        private readonly IValidator<GetUserByIdRequestDto> _validator;

        public UserService(ILogger<UserService> logger, IApiClient apiClient, IValidator<GetUserByIdRequestDto> validator)
        {
            _logger = logger;
            _apiClient = apiClient;
            _validator = validator;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("GetAllAsync");
            var userModels = await _apiClient.GetAllAsync(cancellationToken);
            return userModels.Adapt<IEnumerable<UserResponseDto>>();
        }

        public async Task<UserResponseDto> GetByIdAsync(GetUserByIdRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("GetByIdAsync");

            await _validator.ValidateAndThrowCustomAsync(request, cancellationToken);

            var userModel = await _apiClient.GetByIdAsync(request.Id, cancellationToken);
            if (userModel == null)
            {
                throw new NotFoundException($"User with ID {request.Id} not found!");
            }

            return userModel.Adapt<UserResponseDto>();
        }
    }
}
