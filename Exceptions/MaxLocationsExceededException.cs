namespace DurgerKing.Exceptions;

public class MaxLocationsExceededException : Exception
{
    public MaxLocationsExceededException(int maxLocations) 
        : base($"Max allowed locations is {maxLocations}.") { }
}