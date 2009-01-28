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

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain
{
  public enum CustomerType
  {
    Standard = 0,
    Premium = 1
  }

  [ClassID ("TI_Customer")]
  [Instantiable]
  public abstract class Customer: Person
  {
    public new static Customer NewObject ()
    {
      return NewObject<Customer> ();
    }

    public new static Customer GetObject (ObjectID id)
    {
      return GetObject<Customer> (id);
    }

    protected Customer()
    {
    }

    public abstract CustomerType CustomerType { get; set; }

    public abstract DateTime CustomerSince { get; set; }

    [DBBidirectionalRelation ("Customers")]
    public abstract Region Region { get; set; }

    [DBBidirectionalRelation ("Customer")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
