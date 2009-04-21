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

using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  public abstract class ClientBoundBaseClass : DomainObject
  {
    // types

    // static members and constants

    public static ClientBoundBaseClass GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClientBoundBaseClass> (id);
    }

    public static ClientBoundBaseClass GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.GetObject<ClientBoundBaseClass> (id);
      }
    }

    // member fields

    // construction and disposing

    protected ClientBoundBaseClass ()
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClientBoundBaseClasses")]
    [Mandatory]
    public abstract Client Client { get; set;}
  }
}
