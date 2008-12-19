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
  /// Represents a nullable <see cref="ISecurityPrincipal"/> according to the "Null Object Pattern".
  /// </summary>
  public sealed class NullSecurityPrincipal : ISecurityPrincipal
  {
    public NullSecurityPrincipal ()
    {
    }

    public string User
    {
      get { return null; }
    }

    public ISecurityPrincipalRole Role
    {
      get { return null; }
    }

    public string SubstitutedUser
    {
      get { return null; }
    }

    public ISecurityPrincipalRole SubstitutedRole
    {
      get { return null; }
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}