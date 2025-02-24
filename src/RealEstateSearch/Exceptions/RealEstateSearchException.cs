namespace RealEstateSearch.Exceptions;

public class RealEstateSearchException : Exception
{
    public RealEstateSearchException(string message) : base(message) { }
    public RealEstateSearchException(string message, Exception innerException) : base(message, innerException) { }
}