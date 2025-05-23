using AutoMapper;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Handlers
{
    public class UserRequestAnswerLogic : IUserRequestAnswerLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserRequestAnswerLogic(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserRequestAnswerViewModel> GetUserRequestAnswerByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID turi būti didesnis už nulį.");
            }

            var UserRequestAnswer = await _unitOfWork.UserRequestAnswers.GetByIdAsync(id);
            return _mapper.Map<UserRequestAnswerViewModel>(UserRequestAnswer);
        }

        public async Task<List<UserRequestAnswerViewModel>> GetAllUserRequestAnswersAsync()
        {
            var UserRequestAnswers = await _unitOfWork.UserRequestAnswers.GetAllAsync();
            return _mapper.Map<List<UserRequestAnswerViewModel>>(UserRequestAnswers);
        }

        public async Task<List<UserRequestAnswerViewModel>> GetUserRequestAnswersByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "Vartotojo ID turi būti didesnis už nulį.");
            }

            var UserRequestAnswers = await _unitOfWork.UserRequestAnswers.GetUserRequestAnswersByUserIdAsync(userId);
            return _mapper.Map<List<UserRequestAnswerViewModel>>(UserRequestAnswers);
        }

        public async Task<bool> CreateUserRequestAnswerAsync(UserRequestAnswerViewModel UserRequestAnswer)
        {
            if (UserRequestAnswer == null)
            {
                throw new ArgumentNullException(nameof(UserRequestAnswer), "Vartotojo užklausa negali būti tuščia.");
            }

            var UserRequestAnswerEntity = _mapper.Map<UserRequestAnswer>(UserRequestAnswer);
            return await _unitOfWork.UserRequestAnswers.CreateAsync(UserRequestAnswerEntity);
        }

        public async Task<bool> CreateBulkUserRequestAnswersAsync(List<UserRequestAnswerViewModel> answersVMs, int userId)
        {
            if (answersVMs == null || !answersVMs.Any())
            {
                throw new ArgumentNullException(nameof(answersVMs), "Atsakymų sąrašas negali būti tuščias.");
            }

            var answerEntities = new List<UserRequestAnswer>();
            foreach (var answerVM in answersVMs)
            {
                if (string.IsNullOrWhiteSpace(answerVM.Answer))
                {
                    continue; 
                }
                // Svarbu: UserRequestAnswerViewModel dabar turi turėti FkUseridUser,
                // bet mes jį priskirsime tiesiogiai UserRequestAnswerUser objekte.
                // UserRequestAnswer domeno objektas neturi FkUseridUser.
                var entity = _mapper.Map<UserRequestAnswer>(answerVM); 
                
                // UserRequestAnswerUser objektas bus sukurtas repozitorijoje arba čia,
                // jei repozitorija priima sudėtingesnę struktūrą.
                // Dabar tiesiog perduodame userId į repozitoriją kartu su atsakymais.
                answerEntities.Add(entity);
            }

            if (!answerEntities.Any()) return false;

            // Pakeičiame kvietimą, kad repozitorija žinotų, kuriam vartotojui priskirti
            return await _unitOfWork.UserRequestAnswers.CreateBulkUserRequestAnswersWithUserAsync(answerEntities, userId);
        }
        public async Task<bool> UpdateUserRequestAnswerAsync(UserRequestAnswerViewModel UserRequestAnswer)
        {
            if (UserRequestAnswer == null)
            {
                throw new ArgumentNullException(nameof(UserRequestAnswer), "Vartotojo užklausa negali būti tuščia.");
            }

            var UserRequestAnswerEntity = _mapper.Map<UserRequestAnswer>(UserRequestAnswer);
            return await _unitOfWork.UserRequestAnswers.UpdateAsync(UserRequestAnswerEntity);
        }

        public async Task<bool> DeleteUserRequestAnswerAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID turi būti didesnis už nulį.");
            }

            return await _unitOfWork.UserRequestAnswers.DeleteAsync(id);
        }

        public async Task<bool> CheckUserRequestAnswerDataAsync(UserRequestAnswerViewModel UserRequestAnswer)
        {
            // Validacijos logika pagal diagramą
            if (UserRequestAnswer == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(UserRequestAnswer.Answer))
            {
                return false;
            }
        
            return true;
        }
    }
}