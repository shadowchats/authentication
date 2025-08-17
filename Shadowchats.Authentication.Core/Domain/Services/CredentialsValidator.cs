using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Domain.Services;

public class CredentialsValidator : ICredentialsValidator
{
    public void EnsureCredentialsValidity(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 16)
            throw new InvariantViolationException("Password is invalid; detail: length less than 16.");

        bool hasLower = false, hasUpper = false, hasDigit = false, hasSpecial = false;
        foreach (var @char in password)
        {
            if (!hasLower && @char is >= 'a' and <= 'z')
                hasLower = true;
            else if (!hasUpper && @char is >= 'A' and <= 'Z')
                hasUpper = true;
            else if (!hasDigit && char.IsDigit(@char))
                hasDigit = true;
            else if (!hasSpecial && SpecialCharacters.Contains(@char))
                hasSpecial = true;

            if (hasLower && hasUpper && hasDigit && hasSpecial)
                return;
        }
        
        var missingGroups = new List<string>();
        if (!hasLower)
            missingGroups.Add("[a-z]");
        if (!hasUpper)
            missingGroups.Add("[A-Z]");
        if (!hasDigit)
            missingGroups.Add("[0-9]");
        if (!hasSpecial)
            missingGroups.Add("[!@#$%\\^&*()_+\\-=\\[\\]{}|;':\",./\\\\<>?]");

        throw new InvariantViolationException($"Password is invalid; detail: missing characters from regular expression groups {string.Join(", ", missingGroups)}.");
    }
    
    private static readonly HashSet<char> SpecialCharacters = new("!@#$%^&*()_+-=[]{}|;':\",./\\<>?");
}