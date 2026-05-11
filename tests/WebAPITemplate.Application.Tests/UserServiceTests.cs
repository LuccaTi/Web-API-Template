using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPITemplate.Application.DTOs.Requests;
using WebAPITemplate.Application.Interfaces.Clients;
using WebAPITemplate.Application.Mappings;
using WebAPITemplate.Application.Models;
using WebAPITemplate.Application.Services;

namespace WebAPITemplate.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IApiClient> _mockApiClient;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly Mock<IValidator<GetUserByIdRequestDto>> _mockValidator;
    private readonly UserService _sut; // SUT = System Under Test.

    public UserServiceTests()
    {
        _mockApiClient = new Mock<IApiClient>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockValidator = new Mock<IValidator<GetUserByIdRequestDto>>();

        MappingConfig.RegisterMappings();

        _sut = new UserService(_mockLogger.Object, _mockApiClient.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExistsAndRequestIsValid_ShouldReturnUserDto()
    {
        // 1.ARRANGE
        var request = new GetUserByIdRequestDto { Id = 1 };

        var mockUserModel = new UserModel
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com"
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockApiClient
            .Setup(client => client.GetByIdAsync(request.Id))
            .ReturnsAsync(mockUserModel);

        // 2.ACT
        var result = await _sut.GetByIdAsync(request);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(mockUserModel.Id, result.Id);
        Assert.Equal(mockUserModel.Name, result.FullName);
        Assert.Equal(mockUserModel.Email, result.EmailAddress);

        _mockApiClient.Verify(client => client.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenRequestIsInvalid_ShouldThrowValidationException()
    {
        // 1.ARRANGE
        var request = new GetUserByIdRequestDto { Id = -5 };

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Id", "Id must be greater than zero.")
        };

        var invalidResult = new FluentValidation.Results.ValidationResult(validationFailures);

        _mockValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _sut.GetByIdAsync(request));

        Assert.NotEmpty(exception.Errors);

        _mockApiClient.Verify(client => client.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }
}