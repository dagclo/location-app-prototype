using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace rest_api.Controllers
{
    [Route("location")]
    [ApiController]
    public class GeoTagController : ControllerBase
    {
        private readonly ILocationStore _locationStore;
        private readonly ILogger<GeoTagController> _logger;

        public GeoTagController(ILocationStore locationStore, ILogger<GeoTagController> logger)
        {
            _locationStore = locationStore;
            _logger = logger;
        }

        // GET api/values
        [HttpGet("findByCoordinates")]
        public async Task<IEnumerable<string>> Search([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusInMiles, [FromQuery] int limit = 10)
        {
            _logger.LogDebug(" searching for location {miles} from {lat},{long}", radiusInMiles, latitude, longitude);
            IEnumerable<Location> locations = await _locationStore.SearchAsync(latitude, longitude, radiusInMiles, limit);
            return locations.Select(l => l.Id);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> Get(string id)
        {
            _logger.LogDebug(" retrieving {location}", id.ToString());
            Location location = await _locationStore.GetAsync(id);
            return location;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<dynamic>> Post([FromBody] Location location)
        {
            _logger.LogDebug("storing location {location}", location.ToString());
            
            string storeId = await _locationStore.StoreAsync(location);

            return new { location_id = storeId };
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            _logger.LogDebug(" deleting {location}", id.ToString());

            bool deleted = await _locationStore.DeleteAsync(id);

            return deleted;
        }
    }
}
