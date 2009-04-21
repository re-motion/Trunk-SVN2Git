// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  /// <summary>The base interface for all business objects that need security features.</summary>
  public interface ISecurableObject
  {
    /// <summary>Gets the <see cref="IObjectSecurityStrategy"/> used by that business object.</summary>
    /// <remarks>Primarily used by a <see cref="T:Remotion.Security.SecurityClient"/> to dispatch security checks.</remarks>
    /// <returns>Returns the <see cref="IObjectSecurityStrategy"/>.</returns>
    IObjectSecurityStrategy GetSecurityStrategy ();

    /// <summary>Gets the <see cref="Type"/> representing the <see cref="ISecurableObject"/> in the security infrastructure.</summary>
    /// <returns>Return a <see cref="Type"/> object.</returns>
    Type GetSecurableType ();
  }
}
