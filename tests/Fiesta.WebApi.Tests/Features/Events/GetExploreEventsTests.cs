using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Events;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Domain.Entities;
using FluentAssertions;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class GetExploreEventsTests : WebAppTestBase
    {
        public GetExploreEventsTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenUserLocatedAtBratislava_WhenExploringEventMax100KmAway_Only100KmFarEventsAreReturned()
        {
            var bratislavaLocation = new LocationObject(48.148598, 17.107748);
            var senecLocation = new LocationObject(48.217035, 17.406143);
            var kosiceLocation = new LocationObject(48.717110, 21.259781);

            var bratislavaEvent = ArrangeDb.SeedEvent(null, x => x.SetLocation(bratislavaLocation));
            var senecEvent = ArrangeDb.SeedEvent(null, x => x.SetLocation(senecLocation));
            var kosiceEvent = ArrangeDb.SeedEvent(null, x => x.SetLocation(kosiceLocation));
            await ArrangeDb.SaveChangesAsync();

            var body = new GetExploreEvents.Query
            {
                MaxDistanceFilter = 100,
                CurrentUserLocation = new LatLon(bratislavaLocation.Latitude, bratislavaLocation.Longitude),
                OnlineFilter = GetExploreEvents.OnlineFilter.OfflineOnly,
            };
            var response = await Client.PostAsJsonAsync($"/api/events/explore", body);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<QueryResponse<GetExploreEvents.ResponseDto>>();
            content.Entries.Select(x => x.Id).Should().BeEquivalentTo(new[] { bratislavaEvent.Id, senecEvent.Id });
        }
    }
}
