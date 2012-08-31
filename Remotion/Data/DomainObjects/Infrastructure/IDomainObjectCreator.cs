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
    /// Creates a new <see cref="DomainObject"/> instance and initializes it with the given <paramref name="objectID"/> and 
    /// <paramref name="clientTransaction"/>. The object is enlisted with the transaction, but no <see cref="DataContainer"/> is created for it.
    /// The instance is created without a constructor being called.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> to assign to the object.</param>
    /// <param name="clientTransaction">The transaction to create the object reference with. The reference is automatically enlisted in the given
    /// transaction. If the transaction is a binding transaction, the reference is automatically bound to it.</param>
    /// <returns>A <see cref="DomainObject"/> instance with the given <paramref name="objectID"/> that is enlisted in the given transaction.</returns>
    DomainObject CreateObjectReference (ObjectID objectID, ClientTransaction clientTransaction);

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the given <paramref name="domainObjectType"/> by calling its constructor.
    /// This method <see cref="ConstructorLookupInfo"/> does not raise the events notmally raised when a <see cref="DomainObject"/> is constructed. 
    /// Use <see cref="ClientTransaction.NewObject"/> to create a <see cref="DomainObject"/> with the right events being fired.
    /// </summary>
    /// <param name="domainObjectType">Type of the domain object.</param>
    /// <param name="constructorParameters">The constructor parameters.</param>
    /// <returns>A <see cref="DomainObject"/> instance of the given <paramref name="domainObjectType"/> with its constructor executed.</returns>
    /// <remarks>
    /// The returned object might be an instance of a proxy type compatible with <paramref name="domainObjectType"/>.
    /// </remarks>
    DomainObject CreateNewObject (Type domainObjectType, ParamList constructorParameters);
  }
}
