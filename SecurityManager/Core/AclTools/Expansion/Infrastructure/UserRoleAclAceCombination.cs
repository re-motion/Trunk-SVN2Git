// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// A tuple of (<see cref="User"/>, <see cref="Role"/>, <see cref="AccessControlList"/>, <see cref="AccessControlEntry"/>).
  /// Returned by the enumerator of <see cref="UserRoleAclAceCombinationFinder"/>.
  /// </summary>
  public class UserRoleAclAceCombination
  {
    public UserRoleAclAceCombination (Role role, AccessControlEntry ace)
    {
      Role = role;
      Ace = ace;
    }

    public Role Role { get; private set; }
    public User User { get { return Role.User; } }
    public AccessControlEntry Ace { get; private set; }
    public AccessControlList Acl { get { return Ace.AccessControlList; } }
  }
}