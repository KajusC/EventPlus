using EventPlus.Server.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IQuestionLogic
    {
        Task<QuestionViewModel> GetQuestionByIdAsync(int id);
        Task<List<QuestionViewModel>> GetAllQuestionsAsync();
        Task<bool> CreateQuestionAsync(QuestionViewModel question);
        Task<bool> UpdateQuestionAsync(QuestionViewModel question);
        Task<bool> DeleteQuestionAsync(int id);
    }
}