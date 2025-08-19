// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IUnitOfWork : IInfrastructureService
{
    void Begin();
    void Commit();
    void Rollback();
}