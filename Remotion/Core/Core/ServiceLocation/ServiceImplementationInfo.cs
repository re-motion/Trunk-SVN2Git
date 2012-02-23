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

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Encapsulates a service implementation type and <see cref="LifetimeKind"/>.
  /// </summary>
  public struct ServiceImplementationInfo
  {
    private readonly Type _implementationType;
    private readonly LifetimeKind _lifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceImplementationInfo"/> struct.
    /// </summary>
    /// <param name="implementationType">The concrete implementation of the service type.</param>
    /// <param name="lifetime">The lifetime of the instances of <paramref name="implementationType"/>.</param>
    public ServiceImplementationInfo (Type implementationType, LifetimeKind lifetime)
    {
      _implementationType = implementationType;
      _lifetime = lifetime;
    }

    /// <summary>
    /// Gets the concrete implementation type of the service.
    /// </summary>
    /// <value>The concrete implementation.</value>
    public Type ImplementationType
    {
      get { return _implementationType; }
    }

    /// <summary>
    /// Gets the lifetime of the instances of <see cref="ImplementationType"/>.
    /// </summary>
    /// <value>The lifetime of the instances.</value>
    public LifetimeKind Lifetime
    {
      get { return _lifetime; }
    }

    /// <inheritdoc />
    public override string ToString ()
    {
      return string.Format ("{{{0}, {1}}}", _implementationType, _lifetime);
    }
  }
}