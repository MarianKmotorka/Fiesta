namespace Fiesta.Application.Features.Events.Common
{
    public class LocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Premise { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AdministrativeAreaLevel1 { get; set; }
        public string AdministrativeAreaLevel2 { get; set; }
        public string PostalCode { get; set; }
        public string GoogleMapsUrl { get; set; }
    }
}
