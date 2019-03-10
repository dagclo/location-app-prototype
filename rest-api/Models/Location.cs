using System;
using System.Collections.Generic;
using rest_api.LocationStore;

namespace rest_api.Controllers
{
    public class Location
    {
        public Location(string id, string name, string text, double latitude, double longitude)
        {
            Id = id;
            this.Text = text;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Name = name;
        }

        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }

        public MongoDBLocationDocument ToDocument()
        {
            return new MongoDBLocationDocument(Id, Name, Text, Longitude, Latitude);
        }
    }
}