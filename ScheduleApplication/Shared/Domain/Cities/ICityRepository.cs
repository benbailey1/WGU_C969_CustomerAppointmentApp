using ScheduleApplication.Shared.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleApplication.Shared.Domain.Cities
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<City> GetCityByIdAsync(int cityId);
        Task<int> AddCityAsync(City city);
        Task UpdateCityAsync(City city);
        Task DeleteCityAsync(int cityId);
    }
}
