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
using Remotion.Collections;

namespace Remotion.Security
{
  /// <summary>
  /// Cache for the <see cref="AccessType"/> array, using the <see cref="SecurityContext"/> and the user name (<see cref="string"/>) as key.
  /// These are used as parameters for each call to the <see cref="ISecurityProvider.GetAccess"/> method of <see cref="ISecurityProvider"/>.
  /// </summary>
  /// <remarks><note type="inotes">Implementations are free to implement their own best practice for keeping the cache up to date.</note></remarks>
  public interface IGlobalAccessTypeCacheProvider : INullObject
  {
    /// <summary>
    /// Gets the <see cref="ICache{TKey,TValue}"/> for the <see cref="SecurityContext"/> and user name (<see cref="string"/>) key pair.
    /// </summary>
    /// <returns>The <see cref="ICache{T, S}"/> in use.</returns>
    ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache ();
  }
}
