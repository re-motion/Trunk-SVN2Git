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

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderItemWithNewPropertyAccess : DomainObject
  {
    public static OrderItemWithNewPropertyAccess NewObject ()
    {
      return NewObject<OrderItemWithNewPropertyAccess> ().With();
    }

    protected OrderItemWithNewPropertyAccess()
    {
    }

    [DBBidirectionalRelation ("OrderItems")]
    public abstract OrderWithNewPropertyAccess Order { get; set; }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess OriginalOrder
    {
      get { return Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItemWithNewPropertyAccess.Order"].GetOriginalValue <OrderWithNewPropertyAccess>(); }
    }
  }
}
