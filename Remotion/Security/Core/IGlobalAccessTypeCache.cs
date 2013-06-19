// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.ServiceLocation;

namespace Remotion.Security
{
  /// <summary>
  /// Cache for the <see cref="AccessType"/> array, using the <see cref="SecurityContext"/> and the <see cref="ISecurityPrincipal"/> as key.
  /// These are used as parameters for each call to the <see cref="ISecurityProvider.GetAccess"/> method of <see cref="ISecurityProvider"/>.
  /// </summary>
  /// <remarks><note type="inotes">Implementations are free to implement their own best practice for keeping the cache up to date.</note></remarks>
  [ConcreteImplementation (
      "Remotion.SecurityManager.Domain.GlobalAccessTypeCache.RevisionBasedGlobalAccessTypeCache, Remotion.SecurityManager, "
      + "Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>",
      ignoreIfNotFound: true,
      Position = 0,
      Lifetime = LifetimeKind.Singleton)]
  [ConcreteImplementation (typeof (NullGlobalAccessTypeCache), Position = 1, Lifetime = LifetimeKind.Singleton)]
  public interface IGlobalAccessTypeCache : ICache<GlobalAccessTypeCacheKey, AccessType[]>
  {
    //TODO RM-5521: test IoC
  }
}