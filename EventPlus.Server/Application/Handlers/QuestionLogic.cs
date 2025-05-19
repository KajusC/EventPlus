using AutoMapper;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Handlers
{
    public class QuestionLogic : IQuestionLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionLogic(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<QuestionViewModel>> GetAllQuestionsAsync()
        {
            var questions = await _unitOfWork.Questions.GetAllAsync();
            return _mapper.Map<List<QuestionViewModel>>(questions);
        }

        public async Task<QuestionViewModel> GetQuestionByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID turi būti didesnis už nulį.");
            }

            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            return _mapper.Map<QuestionViewModel>(question);
        }

        public async Task<bool> CreateQuestionAsync(QuestionViewModel question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            var questionEntity = _mapper.Map<Question>(question);
            return await _unitOfWork.Questions.CreateAsync(questionEntity);
        }

        public async Task<bool> UpdateQuestionAsync(QuestionViewModel question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            var questionEntity = _mapper.Map<Question>(question);
            return await _unitOfWork.Questions.UpdateAsync(questionEntity);
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID turi būti didesnis už nulį.");
            }

            return await _unitOfWork.Questions.DeleteAsync(id);
        }
    }
}