using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IOrganiserLogic
    {
        Task<OrganiserViewModel> GetOrganiserByIdAsync(int id);
        Task<List<OrganiserViewModel>> GetAllOrganisersAsync();
        Task<OrganiserViewModel> CreateOrganiserAsync(OrganiserViewModel organiser);
        Task<bool> UpdateOrganiserAsync(OrganiserViewModel organiser);
        Task<bool> DeleteOrganiserAsync(int id);
    }
} 