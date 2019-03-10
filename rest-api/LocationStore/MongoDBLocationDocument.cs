using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;
using rest_api.Controllers;

namespace rest_api.LocationStore
{
    public class MongoDBLocationDocument
    {
        public MongoDBLocationDocument(string id, string text, double longitude, double latitude)
        {
            Id = id;
            Text = text;
            this.Coordinates = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(longitude, latitude));
        }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("coordinates")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public Location ToLocation()
        {
            return new Location(Id, Text, Coordinates.Coordinates.Latitude, Coordinates.Coordinates.Longitude);
        }
    }
}
