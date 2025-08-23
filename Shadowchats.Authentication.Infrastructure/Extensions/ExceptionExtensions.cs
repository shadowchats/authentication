using System.Diagnostics;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Extensions;

internal static class ExceptionExtensions
{
    public static string GetLocation(this Exception exception)
    {
        foreach (var frame in new StackTrace(exception, true).GetFrames())
            if (frame.GetMethod()?.DeclaringType?.Assembly.FullName?.Contains(SolutionPrefix) ?? false)
                return $"{frame.GetFileName()} : {frame.GetFileLineNumber()}";

        throw new BugException("Exception location not found.");
    }

    private const string SolutionPrefix = "Shadowchats.Authentication";
}