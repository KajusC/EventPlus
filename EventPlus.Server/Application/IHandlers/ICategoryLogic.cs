using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface ICategoryLogic
    {
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryViewModel category);
        Task<bool> UpdateCategoryAsync(CategoryViewModel category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
