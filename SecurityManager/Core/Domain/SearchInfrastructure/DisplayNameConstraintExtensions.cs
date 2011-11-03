// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  internal static class DisplayNameConstraintExtensions
  {
    public static IQueryable<Tenant> Apply (this IQueryable<Tenant> tenants, DisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("tenants", tenants);

      if (HasConstraint (constraint))
        return tenants.Where (t => t.Name.Contains (constraint.Value));

      return tenants;
    }

    public static IQueryable<Group> Apply (this IQueryable<Group> groups, DisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("groups", groups);

      if (HasConstraint (constraint))
        return groups.Where (g => g.Name.Contains (constraint.Value) || g.ShortName.Contains (constraint.Value));

      return groups;
    }

    public static IQueryable<User> Apply (this IQueryable<User> users, DisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("users", users);

      if (HasConstraint (constraint))
        return users.Where (u => u.LastName.Contains (constraint.Value) || u.FirstName.Contains (constraint.Value));

      return users;
    }

    public static IQueryable<Position> Apply (this IQueryable<Position> positions, DisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("positions", positions);

      if (HasConstraint (constraint))
        return positions.Where (t => t.Name.Contains (constraint.Value));

      return positions;
    }

    public static IQueryable<GroupType> Apply (this IQueryable<GroupType> groupTypes, DisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("groupTypes", groupTypes);

      if (HasConstraint (constraint))
        return groupTypes.Where (t => t.Name.Contains (constraint.Value));

      return groupTypes;
    }

    private static bool HasConstraint (DisplayNameConstraint constraint)
    {
      return constraint != null && !String.IsNullOrEmpty (constraint.Value);
    }
  }
}