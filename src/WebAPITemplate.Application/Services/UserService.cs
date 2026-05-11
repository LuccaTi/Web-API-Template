using WebAPITemplate.Application.Interfaces.Services;
using System.Diagnostics.Metrics;
using WebAPITemplate.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using WebAPITemplate.Application.DTOs.Responses;
using Mapster;
using WebAPITemplate.Domain.Exceptions;
using FluentValidation;
using WebAPITemplate.Application.DTOs.Requests;

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

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            _logger.LogDebug("GetAllAsync");
            var userModels = await _apiClient.GetAllAsync();
            return userModels.Adapt<IEnumerable<UserResponseDto>>();
        }

        public async Task<UserResponseDto> GetByIdAsync(GetUserByIdRequestDto request)
        {
            _logger.LogDebug("GetByIdAsync");

            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var userModel = await _apiClient.GetByIdAsync(request.Id);
            if(userModel == null)
            {
                throw new NotFoundException($"User with ID {request.Id} not found!");
            }

            return userModel.Adapt<UserResponseDto>();
        }
    }
}
