using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.Jobs.RevokeExpiredSessions;

public record RevokeExpiredSessionsJob : ICommand<NoResult>;