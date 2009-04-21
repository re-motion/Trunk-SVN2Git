// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [SecondStorageGroupAttribute]
  [Instantiable]
  public abstract class Official : DomainObject
  {
    public static Official NewObject()
    {
      return DomainObject.NewObject<Official> ();
    }

    public static Official GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Official> (id);
    }

    protected Official()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract OrderPriority ResponsibleForOrderPriority { get; set; }

    public abstract CustomerType ResponsibleForCustomerType { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
