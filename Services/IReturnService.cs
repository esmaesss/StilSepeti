using StilSepetiApp.DTO;
using StilSepetiApp.Enums;

namespace StilSepetiApp.Services
{
    public interface IReturnService
    {
        Task<List<ReturnRequestdto>> GetAllAsync();
        Task<ServiceResult> UpdateStatusAsync(int id, ReturnStatus newStatus);
    }
}
