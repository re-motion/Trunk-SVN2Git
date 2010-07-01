// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private ClientTransactionMock _parentTransaction;
    private ClientTransaction _subTransaction;
    private IPersistenceStrategy _persistenceStrategy;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransaction = new ClientTransactionMock ();
      _subTransaction = _parentTransaction.CreateSubTransaction ();
      _persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (_subTransaction);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "In order for the subtransaction persistence strategy to work correctly, the parent transaction needs to be read-only. Use "
        + "ClientTransaction.CreateSubTransaction() to create a subtransaction and automatically set the parent transaction read-only.\r\n"
        + "Parameter name: parentTransaction")] 
    public void Initialization_ThrowsWhenParentTransactionWriteable ()
    {
      var writeableParentTransaction = new ClientTransactionMock();
      new SubPersistenceStrategy (MockRepository.GenerateStub<IDataManager> (), writeableParentTransaction);
    }

    [Test]
    public void ParentTransaction ()
    {
      Assert.That (_persistenceStrategy.ParentTransaction, Is.SameAs (_parentTransaction));
    }

    [Test]
    public void PersistData_NewDataContainer_ClearsDiscardFlagInParent ()
    {
      var instance = _subTransaction.Execute (() => ClassWithAllDataTypes.NewObject ());
      Assert.That (_parentTransaction.DataManager.IsInvalid (instance.ID), Is.True);

      _persistenceStrategy.PersistData (new[] { _subTransaction.Execute (() => instance.InternalDataContainer) });

      Assert.That (_parentTransaction.DataManager.IsInvalid (instance.ID), Is.False);
    }
  }
}