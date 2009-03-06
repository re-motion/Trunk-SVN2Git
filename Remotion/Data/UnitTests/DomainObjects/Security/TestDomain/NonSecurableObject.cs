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
using Remotion.Data.DomainObjects.DataManagement;
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
        return NewObject<NonSecurableObject>();
      }
    }

    protected NonSecurableObject ()
    {
    }

    public DataContainer GetDataContainer ()
    {
      var transaction = HasBindingTransaction ? GetBindingTransaction() : ClientTransaction.Current;
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (transaction, typeof (ClientTransaction), "GetDataContainer", this);
    }

    public abstract string StringProperty { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract NonSecurableObject Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonSecurableObject> Children { get; }
  }
}
