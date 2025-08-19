// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure;

public class GuidGenerator : IGuidGenerator
{
    public Guid Generate() => Guid.NewGuid();
}