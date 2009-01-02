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
    /// Gets a typesafe constructor invoker for the given <see cref="DomainObject"/> type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to construct by the returned object.</typeparam>
    /// <returns>An object that allows construction of an instance of type <typeparamref name="T"/>. The <see cref="DomainObject"/> constructed
    /// by this object will be a completely new instance, with no reference to an existing <see cref="DataContainer"/>.</returns>
    IFuncInvoker<T> GetTypesafeConstructorInvoker<T> () where T : DomainObject;

    /// <summary>
    /// Gets a typesafe constructor invoker for the given <see cref="DomainObject"/> type <paramref name="domainObjectType"/>.
    /// </summary>
    /// <param name="domainObjectType">The <see cref="DomainObject"/> type to construct by the returned object.</param>
    /// <returns>An object that allows construction of an instance of type <paramref name="domainObjectType"/>. The <see cref="DomainObject"/>
    /// constructed by this object will be a completely new instance, with no reference to an existing <see cref="DataContainer"/>.</returns>
    IFuncInvoker<DomainObject> GetTypesafeConstructorInvoker (Type domainObjectType);
  }
}
