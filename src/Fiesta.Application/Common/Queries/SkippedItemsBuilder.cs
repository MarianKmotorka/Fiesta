using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Common.Queries
{
    public static class SkippedItemsBuilder
    {
        public static async Task<SkippedItemsResponse<T>> BuildResponse<T>(this IQueryable<T> dbQuery, SkippedItemsDocument document, CancellationToken cancellationToken)
        {
            var entries = await dbQuery.Skip(document.Skip).Take(document.Take).ToListAsync(cancellationToken);
            var totalCount = await dbQuery.CountAsync();

            return new SkippedItemsResponse<T>(entries)
            {
                Skip = document.Skip,
                Take = document.Take,
                TotalEntries = totalCount
            };
        }
    }
}
