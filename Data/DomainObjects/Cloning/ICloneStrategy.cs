/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Cloning
{
  /// <summary>
  /// Provides an interface for classes determining the details about how <see cref="DomainObjectCloner"/> clones a <see cref="DomainObject"/>.
  /// </summary>
  public interface ICloneStrategy
  {
    /// <summary>
    /// Called when <see cref="DomainObjectCloner"/> encounters a reference that might to be cloned.
    /// </summary>
    /// <param name="sourceReference">The reference on the source object.</param>
    /// <param name="sourceTransaction">The source transaction used for cloning.</param>
    /// <param name="cloneReference">The reference on the cloned object.</param>
    /// <param name="cloneTransaction">The transaction used for the cloned object.</param>
    /// <param name="context">A <see cref="CloneContext"/> that should be used to obtain clones of objects held by <see cref="sourceReference"/>.</param>
    /// <remarks>Implementers can check the <paramref name="sourceReference"/> and set the <paramref name="cloneReference"/> to clones,
    /// original, or empty as needed. In order to get the right clone for a referenced object, the <paramref name="context"/> can be used.
    /// Note that for bidirectional references, <see cref="HandleReference"/> will be called for both sides of the relation if both sides
    /// are cloned. When the  <paramref name="context"/> is used to obtain the clones, no object will be cloned twice.
    /// </remarks>
    void HandleReference (PropertyAccessor sourceReference, ClientTransaction sourceTransaction, 
        PropertyAccessor cloneReference, ClientTransaction cloneTransaction,
        CloneContext context);
  }
}