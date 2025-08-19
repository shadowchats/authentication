// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class AggregateRoot<TAggregateRoot>(Guid guid) : Entity<TAggregateRoot>(guid) where TAggregateRoot : AggregateRoot<TAggregateRoot>;