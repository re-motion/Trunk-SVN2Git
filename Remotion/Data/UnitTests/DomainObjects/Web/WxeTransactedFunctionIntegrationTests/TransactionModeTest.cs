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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class TransactionModeTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void WxeTransactedFunctionCreateRoot ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootTestTransactedFunction (originalScope).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateChildIfParent ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootWithChildTestTransactedFunction (originalScope.ScopedTransaction, new CreateChildIfParentTestTransactedFunction()).Execute (
            Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionNone ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateNoneTestTransactedFunction (originalScope).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewAutoCommit ()
    {
      SetDatabaseModifyable();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new AutoCommitTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (10));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewNoAutoCommit ()
    {
      SetDatabaseModifyable();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).
            Execute (Context);

        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (5));
      }
    }

    [Test]
    public void WxeTransactedFunctionNoneNoAutoCommit ()
    {
      SetDatabaseModifyable();
      SetInt32Property (5, ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (
            Context);

        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransactionScope.CurrentTransaction), Is.EqualTo (10));
      }

      Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (5));
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit ();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        return objectWithAllDataTypes.Int32Property;
      }
    }
  }
}
