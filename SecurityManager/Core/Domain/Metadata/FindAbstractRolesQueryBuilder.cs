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
  public class FindAbstractRolesQueryBuilder : QueryBuilder
  {
    public FindAbstractRolesQueryBuilder ()
        : base ("FindAbstractRoles", typeof (AbstractRoleDefinition))
    {
    }

    public IQueryable<AbstractRoleDefinition> CreateQuery (EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abstractRoles", abstractRoles);

      Expression body = null;
      foreach (EnumWrapper abstractRole in abstractRoles)
      {
        string abstractRoleName = abstractRole.Name;
        var predicateExpression = ((Expression<System.Func<AbstractRoleDefinition, bool>>) (ar => ar.Name == abstractRoleName)).Body;

        if (body == null)
          body = predicateExpression;
        else
          body = Expression.OrElse (body, predicateExpression);
      }

      return DataContext.Entity<AbstractRoleDefinition>().Where (
          Expression.Lambda<System.Func<AbstractRoleDefinition, bool>> (body, Expression.Parameter (typeof (AbstractRoleDefinition), "ar")));
    }
  }
}