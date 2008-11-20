/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  /// <summary>
  /// Assists <see cref="DomainObjectCloner"/> by cloning all objects referenced by a cloned source object as well. This ensures deep cloning
  /// of a whole object graph.
  /// </summary>
  public class CompleteCloneStrategy : ICloneStrategy
  {
    /// <summary>
    /// Sets the <paramref name="cloneReference"/> to hold clones of the objects referenced by <paramref name="sourceReference"/>.
    /// </summary>
    /// <param name="sourceReference">The reference on the source object.</param>
    /// <param name="cloneReference">The reference on the cloned object.</param>
    /// <param name="context">The <see cref="CloneContext"/> that is used to obtain clones of objects held by <paramref name="sourceReference"/>.</param>
    public void HandleReference (PropertyAccessor sourceReference, PropertyAccessor cloneReference, CloneContext context)
    {
      if (sourceReference.PropertyData.Kind == PropertyKind.RelatedObject)
      {
        var originalRelated = (DomainObject) sourceReference.GetValueWithoutTypeCheck ();
        DomainObject cloneRelated = originalRelated != null ? context.GetCloneFor (originalRelated) : null;
        cloneReference.SetValueWithoutTypeCheck (cloneRelated);
      }
      else
      {
        Assertion.IsTrue (sourceReference.PropertyData.Kind == PropertyKind.RelatedObjectCollection);
        var originalRelatedCollection = (DomainObjectCollection) sourceReference.GetValueWithoutTypeCheck ();
        var cloneRelatedCollection = (DomainObjectCollection) cloneReference.GetValueWithoutTypeCheck ();

        foreach (DomainObject originalRelated in originalRelatedCollection)
        {
          DomainObject cloneRelated = context.GetCloneFor (originalRelated);
          cloneRelatedCollection.Add (cloneRelated);
        }
      }
    }
  }
}