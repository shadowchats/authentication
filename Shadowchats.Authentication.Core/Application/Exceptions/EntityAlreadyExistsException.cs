using System.Linq.Expressions;
using System.Reflection;
using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Application.Exceptions;

public class EntityAlreadyExistsException<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
    : Exception where TEntity : Entity<TEntity>
{
    public bool IsConflictOn(Expression<Func<TEntity, TProperty>> propertyExpression) =>
        GetMemberInfo(propertyExpression) == _propertyMember;

    private static MemberInfo GetMemberInfo(Expression<Func<TEntity, TProperty>> expression)
    {
        return expression.Body switch
        {
            MemberExpression memberExpr => memberExpr.Member,
            UnaryExpression { Operand: MemberExpression unaryMember } => unaryMember.Member,
            _ => throw new BugException("Expression is not a valid member expression.")
        };
    }

    private readonly MemberInfo _propertyMember = GetMemberInfo(propertyExpression);
}