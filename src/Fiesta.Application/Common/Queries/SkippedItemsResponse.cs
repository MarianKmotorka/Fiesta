using System.Collections.Generic;

namespace Fiesta.Application.Common.Queries
{
    public class SkippedItemsResponse<T>
    {
        public SkippedItemsResponse(IEnumerable<T> entries)
        {
            Entries = entries;
        }

        public IEnumerable<T> Entries { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int TotalEntries { get; set; }

        public bool HasMore => Skip + Take < TotalEntries;
    }
}
