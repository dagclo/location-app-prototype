using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api.Controllers
{
    public interface ILocationStore
    {
        Task<string> StoreAsync(Location location);
        Task<bool> DeleteAsync(string id);
        Task<Location> GetAsync(string id);
        Task<IEnumerable<Location>> SearchAsync(double latitude, double longitude, double radiusInMiles, int limit);
    }
}