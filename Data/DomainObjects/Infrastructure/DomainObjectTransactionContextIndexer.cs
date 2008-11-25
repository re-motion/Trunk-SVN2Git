/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexing property to access a <see cref="DomainObject"/>'s transaction-dependent context for a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public struct DomainObjectTransactionContextIndexer
  {
    private readonly DomainObject _domainObject;

    public DomainObjectTransactionContextIndexer (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _domainObject = domainObject;
    }

    public IDomainObjectTransactionContext this[ClientTransaction clientTransaction]
    {
      get { return new DomainObjectTransactionContext (_domainObject, clientTransaction); }
    }
  }
}