/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  /// <summary>
  /// Assists in cloning <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCloner
  {
    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Relations are not cloned, foreign key properties default to null.
    /// </summary>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. Note that the actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <remarks>
    /// The clone is created in the current transaction. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public virtual T CreateValueClone<T> (T source)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ObjectID sourceID = source.ID;
      ClientTransaction cloneTransaction = ClientTransactionScope.CurrentTransaction;

      // Use NewObjectFromDataContainer in order to avoid calling a ctor
      DataContainer cloneDataContainer = cloneTransaction.CreateNewDataContainer (sourceID.ClassDefinition.ClassType);
      T clone = (T) RepositoryAccessor.NewObjectFromDataContainer (cloneDataContainer);

      CopyProperties (source, clone, null, null);
      return clone;
    }

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Referenced objects are cloned according to the given strategy.
    /// </summary>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. Note that the actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <param name="strategy">The <see cref="ICloneStrategy"/> to be used when cloning the object's references.</param>
    /// <remarks>
    /// The clone is created in the current transaction. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public T CreateClone<T> (T source, ICloneStrategy strategy)
    where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("strategy", strategy);

      return CreateClone (source, strategy, new CloneContext (this));
    }

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Referenced objects are cloned according to the given strategy, the given context is used instead of creating a new one.
    /// </summary>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. Note that the actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <param name="strategy">The <see cref="ICloneStrategy"/> to be used when cloning the object's references.</param>
    /// <param name="context">The <see cref="CloneContext"/> to be used by the cloner.</param>
    /// <remarks>
    /// The clone is created in the current transaction. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public T CreateClone<T> (T source, ICloneStrategy strategy, CloneContext context)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("strategy", strategy);
      ArgumentUtility.CheckNotNull ("context", context);

      T clone = context.GetCloneFor (source);
      CopyProperties (source, clone, strategy, context);
      return clone;
    }

    private void CopyProperties<T> (T source, T clone, ICloneStrategy strategy, CloneContext context)
        where T : DomainObject
    {
      ClientTransaction sourceTransaction = source.GetNonNullClientTransaction ();
      ClientTransaction cloneTransaction = ClientTransactionScope.CurrentTransaction;

      CopyProperties (source.Properties, sourceTransaction, clone.Properties, cloneTransaction, strategy, context);
    }

    private void CopyProperties (PropertyIndexer sourceProperties, ClientTransaction sourceTransaction, IEnumerable<PropertyAccessor> cloneProperties, ClientTransaction cloneTransaction, ICloneStrategy strategy, CloneContext context)
    {
      foreach (PropertyAccessor cloneProperty in cloneProperties)
      {
        PropertyAccessor sourceProperty = sourceProperties[cloneProperty.PropertyIdentifier];
        if (cloneProperty.Kind == PropertyKind.PropertyValue)
        {
          object sourceValue = sourceProperty.GetValueWithoutTypeCheckTx (sourceTransaction);
          cloneProperty.SetValueWithoutTypeCheckTx (cloneTransaction, sourceValue);
        }
        else if (strategy != null)
          strategy.HandleReference (sourceProperty, sourceTransaction, cloneProperty, cloneTransaction, context);
      }
    }
  }
}
