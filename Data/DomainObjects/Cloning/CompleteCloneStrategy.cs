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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  /// <summary>
  /// Assists <see cref="DomainObjectCloner"/> by cloning all objects referenced by a cloned source object.
  /// </summary>
  public class CompleteCloneStrategy : ICloneStrategy
  {
    /// <summary>
    /// Sets the <see cref="cloneReference"/> to hold clones of the objects referenced by <see cref="sourceReference"/>.
    /// </summary>
    /// <param name="sourceReference">The reference on the source object.</param>
    /// <param name="sourceTransaction">The source transaction used for cloning.</param>
    /// <param name="cloneReference">The reference on the cloned object.</param>
    /// <param name="cloneTransaction">The transaction used for the cloned object.</param>
    /// <param name="context">A <see cref="CloneContext"/> that should be used to obtain clones of objects held by <see cref="sourceReference"/>.</param>
    public void HandleReference (
        PropertyAccessor sourceReference,
        ClientTransaction sourceTransaction,
        PropertyAccessor cloneReference,
        ClientTransaction cloneTransaction,
        CloneContext context)
    {
      if (sourceReference.Kind == PropertyKind.RelatedObject)
      {
        DomainObject originalRelated = (DomainObject) sourceReference.GetValueWithoutTypeCheckTx (sourceTransaction);
        DomainObject cloneRelated = context.GetCloneFor (originalRelated);
        cloneReference.SetValueWithoutTypeCheckTx (cloneTransaction, cloneRelated);
      }
      else
      {
        Assertion.IsTrue (sourceReference.Kind == PropertyKind.RelatedObjectCollection);
        DomainObjectCollection originalRelatedCollection = (DomainObjectCollection) sourceReference.GetValueWithoutTypeCheckTx (sourceTransaction);
        DomainObjectCollection cloneRelatedCollection = (DomainObjectCollection) cloneReference.GetValueWithoutTypeCheckTx (cloneTransaction);

        foreach (DomainObject originalRelated in originalRelatedCollection)
        {
          DomainObject cloneRelated = context.GetCloneFor (originalRelated);
          cloneRelatedCollection.Add (cloneRelated);
        }
      }
    }
  }
}