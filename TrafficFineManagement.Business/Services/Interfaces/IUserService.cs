using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Users;

namespace TrafficFineManagement.Business.Services.Interfaces;

public interface IUserService
{
    Task<List<UserListItemDto>> GetAllAsync();
    Task<ServiceResult> CreateAsync(UserCreateDto dto);
}