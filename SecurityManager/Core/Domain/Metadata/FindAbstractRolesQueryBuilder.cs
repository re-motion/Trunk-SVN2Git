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

      if (abstractRoles.Length == 0)
        return new AbstractRoleDefinition[0].AsQueryable();

      var abstractRoleNames = (from abstractRole in abstractRoles select abstractRole.Name).ToArray();

      return from ar in DataContext.Entity<AbstractRoleDefinition>()
             where abstractRoleNames.Contains (ar.Name)
             select ar;
    }
  }
}