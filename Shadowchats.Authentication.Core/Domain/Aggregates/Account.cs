using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Domain.Aggregates;

public class Account : AggregateRoot<Account>
{
    public static Account Create(IGuidGenerator guidGenerator, Credentials credentials) => new(guidGenerator.Generate(), credentials);
    
    private Account(Guid guid, Credentials credentials) : base(guid) => Credentials = credentials;

    public Credentials Credentials { get; }
}
