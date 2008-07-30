/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class FindAbstractRolesQueryBuilder
  {
    public FindAbstractRolesQueryBuilder ()
    {
    }

    public IQueryable<AbstractRoleDefinition> CreateQuery (EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNull ("abstractRoles", abstractRoles);

      //var abstractRoleNames = (from abstractRole in abstractRoles select abstractRole.Name).ToArray();

      //return from ar in DataContext.Entity<AbstractRoleDefinition>()
      //       where abstractRoleNames.Contains (ar.Name)
      //       select ar;

      if (abstractRoles.Length == 0)
        return new AbstractRoleDefinition[0].AsQueryable();

      ParameterExpression parameter = Expression.Parameter (typeof (AbstractRoleDefinition), "ar");

      var predicates = from abstractRole in abstractRoles
                       select GetPredicateForAbstractRole (parameter, abstractRole);

      BinaryExpression body = predicates.Aggregate (Expression.OrElse);
      return DataContext.Entity<AbstractRoleDefinition>().Where (Expression.Lambda<System.Func<AbstractRoleDefinition, bool>> (body, parameter));
    }

    private BinaryExpression GetPredicateForAbstractRole (ParameterExpression parameter, EnumWrapper abstractRole)
    {
      return Expression.Equal (
          Expression.MakeMemberAccess (parameter, typeof (AbstractRoleDefinition).GetProperty ("Name")),
          Expression.Constant (abstractRole.Name));
    }
  }
}