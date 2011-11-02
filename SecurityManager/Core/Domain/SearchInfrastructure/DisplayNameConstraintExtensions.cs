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
    public static IQueryable<Group> Apply (this IQueryable<Group> groups, IDisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("groups", groups);

      if (HasConstraint (constraint))
        return groups.Where (g => g.Name.Contains (constraint.Text) || g.ShortName.Contains (constraint.Text));

      return groups;
    }

    public static IQueryable<User> Apply (this IQueryable<User> users, IDisplayNameConstraint constraint)
    {
      ArgumentUtility.CheckNotNull ("users", users);

      if (HasConstraint (constraint))
        return users.Where (g => g.LastName.Contains (constraint.Text) || g.FirstName.Contains (constraint.Text));

      return users;
    }

    private static bool HasConstraint (IDisplayNameConstraint constraint)
    {
      return constraint != null && !String.IsNullOrEmpty (constraint.Text);
    }
  }
}