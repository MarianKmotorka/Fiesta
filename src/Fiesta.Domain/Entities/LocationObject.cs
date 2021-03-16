using Fiesta.Domain.Common;
using System;
using System.Collections.Generic;

namespace Fiesta.Domain.Entities
{
    public class LocationObject : ValueObject
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string Street { get; init; }
        public string StreetNumber { get; init; }
        public string Premise { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string AdministrativeAreaLevel1 { get; init; }
        public string AdministrativeAreaLevel2 { get; init; }
        public string PostalCode { get; init; }
        public string PostalCodeNormalized { get; init; }
        public string GoogleMapsUrl { get; init; }

        public LocationObject(double latitude, double longitude, string street = "", string streetNumber = "",
            string premise = "", string city = "", string state = "", string administrativeAreaLevel1 = "", string administrativeAreaLevel2 = "",
            string postalCode = "", string googleMapsUrl = "")
        {
            if (!ValidateLatitudeAndLongitude(latitude, longitude))
                throw new ArgumentException("Invalid latitude or longitude");

            Latitude = latitude;
            Longitude = longitude;
            Street = street;
            StreetNumber = streetNumber;
            Premise = premise;
            City = city;
            State = state;
            AdministrativeAreaLevel1 = administrativeAreaLevel1;
            AdministrativeAreaLevel2 = administrativeAreaLevel2;
            PostalCode = postalCode;
            PostalCodeNormalized = NormalizePostalCode(postalCode);
            GoogleMapsUrl = googleMapsUrl;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
            yield return Street;
            yield return StreetNumber;
            yield return Premise;
            yield return City;
            yield return State;
            yield return AdministrativeAreaLevel1;
            yield return AdministrativeAreaLevel2;
            yield return PostalCode;
            yield return PostalCodeNormalized;
            yield return GoogleMapsUrl;
        }

        public static bool ValidateLatitudeAndLongitude(double latitude, double longitude)
        {
            return (-90 <= latitude && latitude <= 90) && (-180 <= longitude && longitude <= 180);
        }

        private string NormalizePostalCode(string postalCode)
        {
            return postalCode.Replace(" ", "");
        }
    }
}
