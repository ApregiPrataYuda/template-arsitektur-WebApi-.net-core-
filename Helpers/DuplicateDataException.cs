namespace appOne.Helpers;

public class DuplicateDataException : Exception
{
    public DuplicateDataException(string message) : base(message) { }
}