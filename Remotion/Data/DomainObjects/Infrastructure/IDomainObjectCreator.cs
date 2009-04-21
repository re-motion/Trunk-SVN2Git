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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides a common interface for classes creating new instances of <see cref="DomainObject"/> types.
  /// </summary>
  public interface IDomainObjectCreator
  {
    /// <summary>
    /// Creates a <see cref="DomainObject"/> instance and initializes is from the given existing data container.
    /// </summary>
    /// <param name="dataContainer">The data container to initialize the <see cref="DomainObject"/> with.</param>
    /// <returns>A new <see cref="DomainObject"/> initialized with the given <see cref="DataContainer"/>.</returns>
    DomainObject CreateWithDataContainer (DataContainer dataContainer);

    /// <summary>
    /// Gets a <see cref="ConstructorLookupInfo"/> that can be used to construct a <see cref="DomainObject"/> of the given 
    /// <paramref name="domainObjectType"/>.
    /// </summary>
    /// <param name="domainObjectType">Type of the domain object.</param>
    /// <returns>A <see cref="ConstructorLookupInfo"/> that can be used to instantiate a <see cref="DomainObject"/> of the given type.</returns>
    /// <remarks>
    /// The <see cref="ConstructorLookupInfo"/> returned by this method might not directly represent the given type; instead, it might represent a 
    /// proxy type compatible with <paramref name="domainObjectType"/>.
    /// </remarks>
    ConstructorLookupInfo GetConstructorLookupInfo (Type domainObjectType);
  }
}
