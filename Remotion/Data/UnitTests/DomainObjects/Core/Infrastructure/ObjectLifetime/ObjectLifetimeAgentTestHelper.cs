// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime
{
  public static class ObjectLifetimeAgentTestHelper
  {
    public static void SetCurrentInitializationContext (IObjectInitializationContext objectInitializationContext)
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ObjectLifetimeAgent), "_currentInitializationContext", objectInitializationContext);
    }

    public static void StubCurrentObjectInitializationContext (ClientTransaction clientTransaction, ObjectID objectID)
    {
      var initializationContextStub = new NewObjectInitializationContext (
          objectID,
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager (clientTransaction),
          ClientTransactionTestHelper.GetIDataManager (clientTransaction),
          clientTransaction as BindingClientTransaction);
      SetCurrentInitializationContext (initializationContextStub);
   }

    public static T CallWithInitializationContext<T> (ClientTransaction clientTransaction, ObjectID objectID, Func<T> func)
    {
      StubCurrentObjectInitializationContext (clientTransaction, objectID);
      try
      {
        return func ();
      }
      finally
      {
        SetCurrentInitializationContext (null);
      }
    }

    public static void CallWithInitializationContext (ClientTransaction clientTransaction, ObjectID objectID, Action action)
    {
      StubCurrentObjectInitializationContext (clientTransaction, objectID);
      try
      {
        action ();
      }
      finally
      {
        SetCurrentInitializationContext (null);
      }
    }
  }
}