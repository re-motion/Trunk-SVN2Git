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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public class OrderCollection : ObjectList<Order>
  {
    public OrderCollection ()
    {
    }

    // standard constructor for collections
    public OrderCollection (OrderCollection collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    public new void SetIsReadOnly (bool isReadOnly)
    {
      base.SetIsReadOnly (isReadOnly);
    }
  }
}
