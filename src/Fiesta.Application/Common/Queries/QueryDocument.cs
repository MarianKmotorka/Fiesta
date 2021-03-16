using System.Collections.Generic;

namespace Fiesta.Application.Common.Queries
{
    public class QueryDocument
    {
        private int _page;
        private int _pageSize = 20;

        public int Page
        {
            get => _page;
            set
            {
                if (value < 0)
                    _page = 0;
                else
                    _page = value;
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1)
                    _pageSize = 1;
                else if (value > 300)
                    _pageSize = 300;
                else
                    _pageSize = value;
            }
        }

        public List<Sort> Sorts { get; set; }

        public List<Filter> Filters { get; set; }
    }

    public record Filter(string FieldName, Operation Operation, object FieldValue);

    public record Sort(string FieldName, SortType SortType);
}
