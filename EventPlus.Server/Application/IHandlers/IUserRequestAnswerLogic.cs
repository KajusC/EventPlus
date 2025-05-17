using EventPlus.Server.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IUserRequestAnswerLogic
    {
        Task<UserRequestAnswerViewModel> GetUserRequestAnswerByIdAsync(int id);
        Task<List<UserRequestAnswerViewModel>> GetAllUserRequestAnswersAsync();
        Task<List<UserRequestAnswerViewModel>> GetUserRequestAnswersByUserIdAsync(int userId);
        Task<bool> CreateUserRequestAnswerAsync(UserRequestAnswerViewModel UserRequestAnswer);
        Task<bool> UpdateUserRequestAnswerAsync(UserRequestAnswerViewModel UserRequestAnswer);
        Task<bool> DeleteUserRequestAnswerAsync(int id);
        Task<bool> CheckUserRequestAnswerDataAsync(UserRequestAnswerViewModel UserRequestAnswer);
    }
}