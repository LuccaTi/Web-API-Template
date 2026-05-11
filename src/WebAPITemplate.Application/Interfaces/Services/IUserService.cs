using WebAPITemplate.Application.DTOs.Requests;
using WebAPITemplate.Application.DTOs.Responses;

namespace WebAPITemplate.Application.Interfaces.Services;

public interface IUserService
{
    public Task<IEnumerable<UserResponseDto>> GetAllAsync();
    public Task<UserResponseDto> GetByIdAsync(GetUserByIdRequestDto request);
}
