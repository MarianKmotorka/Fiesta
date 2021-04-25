namespace Fiesta.Application.Common.Queries
{
    public class SkippedItemsDocument
    {
        private int _skip = 0;
        private int _take = 20;

        public int Skip
        {
            get => _skip;
            set
            {
                if (value < 0)
                    _skip = 0;
                else
                    _skip = value;
            }
        }

        public int Take
        {
            get => _take;
            set
            {
                if (value < 1)
                    _take = 1;
                else if (value > 300)
                    _take = 300;
                else
                    _take = value;
            }
        }
    }
}
