using AutoMapper;
using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Handlers
{
    public class OrganiserLogic : IOrganiserLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrganiserLogic(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrganiserViewModel> GetOrganiserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            var organiser = await _unitOfWork.Organisers.GetByIdAsync(id);
            return _mapper.Map<OrganiserViewModel>(organiser);
        }

        public async Task<List<OrganiserViewModel>> GetAllOrganisersAsync()
        {
            var organisers = await _unitOfWork.Organisers.GetAllAsync();
            return _mapper.Map<List<OrganiserViewModel>>(organisers);
        }

        public async Task<OrganiserViewModel> CreateOrganiserAsync(OrganiserViewModel organiser)
        {
            if (organiser == null)
            {
                throw new System.ArgumentNullException(nameof(organiser));
            }

            var organiserEntity = _mapper.Map<Organiser>(organiser);
            await _unitOfWork.Organisers.CreateAsync(organiserEntity);
            await _unitOfWork.SaveAsync();
            
            return _mapper.Map<OrganiserViewModel>(organiserEntity);
        }

        public async Task<bool> UpdateOrganiserAsync(OrganiserViewModel organiser)
        {
            if (organiser == null)
            {
                throw new System.ArgumentNullException(nameof(organiser));
            }

            var organiserEntity = _mapper.Map<Organiser>(organiser);
            var result = await _unitOfWork.Organisers.UpdateAsync(organiserEntity);
            await _unitOfWork.SaveAsync();
            
            return result;
        }

        public async Task<bool> DeleteOrganiserAsync(int id)
        {
            if (id <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            var result = await _unitOfWork.Organisers.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            
            return result;
        }

        public async Task<double> GetOrganiserRating(int organiserId)
        {
            var organiser = await _unitOfWork.Organisers.GetByIdAsync(organiserId);
            var rating = organiser.Rating ?? 0;
            return rating;
        }
    }
} 