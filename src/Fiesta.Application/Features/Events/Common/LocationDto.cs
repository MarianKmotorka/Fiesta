using Fiesta.Domain.Entities;

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

        public static LocationDto Map(LocationObject location)
            => new()
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Street = location.Street,
                StreetNumber = location.StreetNumber,
                Premise = location.Premise,
                City = location.City,
                State = location.State,
                AdministrativeAreaLevel1 = location.AdministrativeAreaLevel1,
                AdministrativeAreaLevel2 = location.AdministrativeAreaLevel2,
                PostalCode = location.PostalCode,
                GoogleMapsUrl = location.GoogleMapsUrl
            };
    }
}
