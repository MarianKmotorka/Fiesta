namespace Fiesta.Application.Common.Queries
{
    public enum Operation
    {
        Equals = 0,
        Contains = 1,
        GreaterThan = 2,
        LessThan = 3,
        GreaterThanOrEqual = 4,
        LessThanOrEqual = 5,
        HasFlag = 6,
        StartsWith = 7,
        EndsWith = 8
    }

    public enum OrderType
    {
        Asc = 0,
        Desc = 1
    }
}
