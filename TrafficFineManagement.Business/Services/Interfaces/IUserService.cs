using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Users;

namespace TrafficFineManagement.Business.Services.Interfaces;

public interface IUserService
{
    Task<List<UserListItemDto>> GetAllAsync();
    Task<ServiceResult> CreateAsync(UserCreateDto dto);
    Task<ServiceResult> SetActiveAsync(string userId, bool isActive, string currentUserId);
    Task<ServiceResult> DeleteAsync(string userId, string currentUserId);
}