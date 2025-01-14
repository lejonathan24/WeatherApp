namespace Weather_Monitoring_Application.Models
{
    public class GeocodeResponse
    {
        public List<Result> Results { get; set; }
        public string Status { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> AddressComponents { get; set; }
        public string FormattedAddress { get; set; }
        public Geometry Geometry { get; set; }
        public string PlaceId { get; set; }
        public PlusCode PlusCode { get; set; }
        public List<string> Types { get; set; }
    }

    public class AddressComponent
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public List<string> Types { get; set; }
    }

    public class Geometry
    {
        public LocationCoords Location { get; set; }
        public string LocationType { get; set; }
        public Viewport Viewport { get; set; }
    }

    public class LocationCoords
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Viewport
    {
        public LocationCoords Northeast { get; set; }
        public LocationCoords Southwest { get; set; }
    }

    public class PlusCode
    {
        public string CompoundCode { get; set; }
        public string GlobalCode { get; set; }
    }

}
