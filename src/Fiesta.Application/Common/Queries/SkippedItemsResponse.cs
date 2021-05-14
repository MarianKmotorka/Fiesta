using System.Collections.Generic;

namespace Fiesta.Application.Common.Queries
{
    public class SkippedItemsResponse<T, TAdditionalData>
    {
        public SkippedItemsResponse(IEnumerable<T> entries, TAdditionalData additionalData)
        {
            Entries = entries;
            AdditionalData = additionalData;
        }

        public IEnumerable<T> Entries { get; set; }

        public TAdditionalData AdditionalData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int TotalEntries { get; set; }

        public bool HasMore => Skip + Take < TotalEntries;
    }

    public class SkippedItemsResponse<T> : SkippedItemsResponse<T, object>
    {
        public SkippedItemsResponse(IEnumerable<T> entries) : base(entries, null)
        {
        }
    }
}
