using System.Collections.Generic;

namespace Fiesta.Application.Common.Queries
{
    public class QueryResponse<T>
    {
        public QueryResponse(IEnumerable<T> entries)
        {
            Entries = entries;
        }

        public IEnumerable<T> Entries { get; set; }

        /// <summary>
        /// Page index starting from zero
        /// </summary>
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int TotalEntries { get; set; }

        public bool HasMore => Page + 1 < TotalPages;

        public int NextPage => Page + 1;
    }
}
