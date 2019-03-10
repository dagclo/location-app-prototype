namespace rest_api.LocationStore
{
    public static class DistanceConversions
    {
        public static double ToMeters(this double miles)
        {
            return miles * 1609.34;
        }
    }
}
