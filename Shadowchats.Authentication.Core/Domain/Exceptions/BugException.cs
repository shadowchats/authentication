namespace Shadowchats.Authentication.Core.Domain.Exceptions;

public class BugException : Exception
{
    public BugException(string message) : base(message) { }
}