// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using JetBrains.Annotations;
using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Domain.ValueObjects;

public class Credentials : ValueObject<Credentials>
{
    [UsedImplicitly]
    private Credentials() { }
    
    public static Credentials Create(IPasswordHasher passwordHasher, string login, string password)
    {
        login = login.Trim();
        password = password.Trim();
        
        EnsureLoginValidity(login);
        EnsurePasswordValidity(password);
        
        return new Credentials(login, passwordHasher.Hash(password));
    }
    
    private static void EnsureLoginValidity(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new InvariantViolationException("Login is empty.");
        switch (login.Length)
        {
            case < 4:
                throw new InvariantViolationException("Login is invalid; detail: length less than 4.");
            case > 16:
                throw new InvariantViolationException("Login is invalid; detail: length greater than 16.");
        }
    }
    
    private static void EnsurePasswordValidity(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvariantViolationException("Password is empty.");
        switch (password.Length)
        {
            case < 16:
                throw new InvariantViolationException("Password is invalid; detail: length less than 16.");
            case > 64:
                throw new InvariantViolationException("Password is invalid; detail: length greater than 64.");
        }

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

    private Credentials(string login, string passwordHash)
    {
        Login = login;
        PasswordHash = passwordHash;
    }
    
    public bool VerifyPassword(IPasswordHasher hasher, string password) => hasher.Verify(PasswordHash, password);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Login;
        yield return PasswordHash;
    }
    
    public string Login { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
}