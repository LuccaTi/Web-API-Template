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
    private readonly Mock<IValidator<GetUserByIdRequestDto>> _mockGetValidator;
    private readonly UserService _sut; // SUT = System Under Test.

    public UserServiceTests()
    {
        _mockApiClient = new Mock<IApiClient>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockGetValidator = new Mock<IValidator<GetUserByIdRequestDto>>();

        MappingConfig.RegisterMappings();

        _sut = new UserService(_mockLogger.Object, _mockApiClient.Object, _mockGetValidator.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenOperationIsCancelled_ShouldThrowTaskCancelledException()
    {
        // 1.ARRANGE
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _mockApiClient.Setup(client => client.GetAllAsync(cancellationToken))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));

        cts.Cancel();

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.GetAllAsync(cancellationToken));

        _mockApiClient.Verify(
            client => client.GetAllAsync(It.Is<CancellationToken>(ct => ct == cancellationToken)),
            Times.Once);
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

        var cancellationToken = CancellationToken.None;

        _mockGetValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockApiClient
            .Setup(client => client.GetByIdAsync(request.Id, cancellationToken))
            .ReturnsAsync(mockUserModel);

        // 2.ACT
        var result = await _sut.GetByIdAsync(request, cancellationToken);

        // 3.ASSERT
        Assert.NotNull(result);
        Assert.Equal(mockUserModel.Id, result.Id);
        Assert.Equal(mockUserModel.Name, result.FullName);
        Assert.Equal(mockUserModel.Email, result.EmailAddress);

        _mockApiClient.Verify(client => client.GetByIdAsync(1, cancellationToken), Times.Once);
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

        var cancellationToken = CancellationToken.None;

        _mockGetValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);

        // 2.ACT & 3.ASSERT
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _sut.GetByIdAsync(request, cancellationToken));

        Assert.NotEmpty(exception.Errors);

        _mockApiClient.Verify(client => client.GetByIdAsync(It.IsAny<long>(), cancellationToken), Times.Never);
    }
}