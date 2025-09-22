namespace GameStore.Domain.Exceptions;
public class UnauthenticatedException(string message) : Exception(message)
{
}
