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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class NonSecurableObject : DomainObject
  {
    public static NonSecurableObject NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<NonSecurableObject>().With();
      }
    }

    protected NonSecurableObject ()
    {
    }

    public DataContainer GetDataContainer ()
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (ClientTransaction, typeof (ClientTransaction), "GetDataContainer", this);
    }

    public abstract string StringProperty { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract NonSecurableObject Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonSecurableObject> Children { get; }
  }
}
