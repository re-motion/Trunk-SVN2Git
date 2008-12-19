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

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="ISecurityPrincipal"/> interface represents a user, and optionally his active role and the user for whom he's acting as a substitue.
  /// </summary>
  public interface ISecurityPrincipal : INullObject
  {
    /// <summary>
    /// Gets the name of the user. The <see cref="User"/> must always be specified.
    /// </summary>
    string User { get; }

    /// <summary>
    /// Gets the active role of the user. The <see cref="Role"/> is optional.
    /// </summary>
    ISecurityPrincipalRole Role { get; }

    /// <summary>
    /// Gets the name of the user that is being substitued. 
    /// The <see cref="SubstitutedUser"/> must be specified if a <see cref="SubstitutedRole"/> is specified as well.
    /// </summary>
    string SubstitutedUser { get; }

    /// <summary>
    /// Gets the role that is being substituted. The <see cref="SubstitutedRole"/> is optional.
    /// </summary>
    ISecurityPrincipalRole SubstitutedRole { get; }
  }
}