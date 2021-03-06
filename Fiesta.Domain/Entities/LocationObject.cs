using Fiesta.Domain.Common;
using System;
using System.Collections.Generic;

namespace Fiesta.Domain.Entities
{
    public class LocationObject : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string Premise { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string AdministrativeAreaLevel1 { get; private set; }
        public string AdministrativeAreaLevel2 { get; private set; }
        public string PostalCode { get; private set; }
        public string PostalCodeNormalized { get; private set; }
        public string GoogleMapsUrl { get; private set; }

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

        private bool ValidateLatitudeAndLongitude(double latitude, double longitude)
        {
            return (-90 <= latitude && latitude <= 90) && (-180 <= longitude && longitude <= 180);
        }

        private string NormalizePostalCode(string postalCode)
        {
            return postalCode.Replace(" ", "");
        }
    }
}
