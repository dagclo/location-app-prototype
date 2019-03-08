using System;
using System.Collections.Generic;
using rest_api.LocationStore;

namespace rest_api.Controllers
{
    public class Location
    {
        public Location(string id, string text, double latitude, double longitude)
        {
            Id = id;
            this.Text = text;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Text { get; set; }

        public MongoDBLocationDocument ToDocument()
        {
            return new MongoDBLocationDocument(Id, Text, Longitude, Latitude);
        }
    }
}