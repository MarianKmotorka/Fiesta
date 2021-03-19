using Fiesta.Application.Common.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Fiesta.Application.Tests
{
    public class OptionalTests
    {
        public class TestDto
        {
            public Optional<string> Name { get; set; }

            public Optional<int> Age { get; set; }

            public Optional<LatLng> Location { get; set; }

            public class LatLng
            {
                public int Lat { get; set; }

                public int Lng { get; set; }
            }
        }

        [Fact]
        public void GivenSimpleJson_WhenParsingToContract_JsonIsCorrectlyParsed()
        {
            var json = "{'name':'Adam'}";
            var dto = JsonConvert.DeserializeObject<TestDto>(json);

            dto.Name.HasValue.Should().BeTrue();
            dto.Name.Value.Should().Be("Adam");
            dto.Age.HasValue.Should().BeFalse();
            dto.Location.HasValue.Should().BeFalse();
        }

        [Fact]
        public void GivenNullValuesJson_WhenParsingToContract_JsonIsCorrectlyParsed()
        {
            var json = "{'location': null}";
            var dto = JsonConvert.DeserializeObject<TestDto>(json);

            dto.Name.HasValue.Should().BeFalse();
            dto.Age.HasValue.Should().BeFalse();
            dto.Location.HasValue.Should().BeTrue();
            dto.Location.Value.Should().BeNull();
        }

        [Fact]
        public void GivenComplexJson_WhenParsingToContract_JsonIsCorrectlyParsed()
        {
            var json = @"{
                            'age': 18,
                            'location': { 'lat': 30, 'lng': 40 }
                        }";

            var dto = JsonConvert.DeserializeObject<TestDto>(json);

            dto.Name.HasValue.Should().BeFalse();
            dto.Age.HasValue.Should().BeTrue();
            dto.Age.Value.Should().Be(18);
            dto.Location.HasValue.Should().BeTrue();
            dto.Location.Value.Should().BeEquivalentTo(new TestDto.LatLng { Lat = 30, Lng = 40 });
        }
    }
}
