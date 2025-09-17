// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Linq.Expressions;
using System.Reflection;
using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Application.Exceptions;

public class EntityAlreadyExistsException<TEntity, TProperty>
    : Exception where TEntity : Entity<TEntity>
{
    public EntityAlreadyExistsException(Expression<Func<TEntity, TProperty>> propertyExpression) =>
        _propertyMember = GetMemberInfo(propertyExpression);
    
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

    private readonly MemberInfo _propertyMember;
}