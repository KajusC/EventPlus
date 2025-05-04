using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
    public class CategoryLogic : ICategoryLogic
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryLogic(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateCategoryAsync(CategoryViewModel category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var categoryEntity = _mapper.Map<Category>(category);
            return await _unitOfWork.Categories.CreateAsync(categoryEntity);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _unitOfWork.Categories.DeleteAsync(id);
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<List<CategoryViewModel>>(categories);
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var categoryEntity = await _unitOfWork.Categories.GetByIdAsync(id);
            return _mapper.Map<CategoryViewModel>(categoryEntity);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryViewModel category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            var categoryEntity = _mapper.Map<Category>(category);
            return await _unitOfWork.Categories.UpdateAsync(categoryEntity);
        }
    }
}
