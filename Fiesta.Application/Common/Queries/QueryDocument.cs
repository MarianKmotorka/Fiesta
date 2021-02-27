using System.Collections.Generic;

namespace Fiesta.Application.Common.Queries
{
    public class QueryDocument
    {
        public int Page { get; set; }
        public int PageSize { get; set; } = 20;
        public List<Sort> Sorts { get; set; }
        public List<Filter> Filters { get; set; }
    }

    public record Filter(string FieldName, Operation Operation, object FieldValue);

    public record Sort(string FieldName, OrderType Order);
}
