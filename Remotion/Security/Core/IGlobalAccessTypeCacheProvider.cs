// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
  /// Cache for the <see cref="AccessType"/> array, using the <see cref="SecurityContext"/> and the <see cref="ISecurityPrincipal"/> as key.
  /// These are used as parameters for each call to the <see cref="ISecurityProvider.GetAccess"/> method of <see cref="ISecurityProvider"/>.
  /// </summary>
  /// <remarks><note type="inotes">Implementations are free to implement their own best practice for keeping the cache up to date.</note></remarks>
  public interface IGlobalAccessTypeCacheProvider : INullObject
  {
    /// <summary>
    /// Gets the <see cref="ICache{TKey,TValue}"/> for the <see cref="ISecurityContext"/> and <see cref="ISecurityPrincipal"/> key pair.
    /// </summary>
    /// <returns>The <see cref="ICache{T, S}"/> in use.</returns>
    ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> GetCache ();
  }
}
