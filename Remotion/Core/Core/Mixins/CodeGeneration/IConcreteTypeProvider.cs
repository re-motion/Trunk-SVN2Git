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
using Remotion.Mixins.Context;
using Remotion.Reflection;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a concrete, mixed type for a given <see cref="ClassContext"/>.
  /// </summary>
  /// <remarks>
  /// This interface is mostly for internal reasons, users should use 
  /// <see cref="ObjectFactory.Create(System.Type,Remotion.Reflection.ParamList,object[])"/> or <see cref="TypeFactory.GetConcreteType(System.Type)"/> 
  /// instead.
  /// </remarks>
  public interface IConcreteTypeProvider
  {
    /// <summary>
    /// Gets a concrete mixed type for the given target class configuration.
    /// </summary>
    /// <param name="classContext">The <see cref="ClassContext"/> holding the mixin configuration for the target class.</param>
    /// <returns>A concrete type with all mixins from <paramref name="classContext"/> mixed in.</returns>
    Type GetConcreteType (ClassContext classContext);

    /// <summary>
    /// Gets an <see cref="IConstructorLookupInfo"/> object that can be used to construct the concrete mixed type for the given target class
    /// configuration either from the cache or by generating it.
    /// </summary>
    /// <param name="classContext">The <see cref="ClassContext"/> holding the mixin configuration for the target class.</param>
    /// <param name="allowNonPublic">If set to <see langword="true"/>, the result object supports calling non-public constructors. Otherwise,
    /// only public constructors are allowed.</param>
    /// <returns>
    /// An <see cref="IConstructorLookupInfo"/> instance instantiating the same type <see cref="GetConcreteType"/> would have returned for the given
    /// <paramref name="classContext"/>.
    /// </returns>
    IConstructorLookupInfo GetConstructorLookupInfo (ClassContext classContext, bool allowNonPublic);
  }
}