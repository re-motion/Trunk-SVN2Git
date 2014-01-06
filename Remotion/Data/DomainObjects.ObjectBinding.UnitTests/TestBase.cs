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
using System.Transactions;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  public abstract class TestBase //: StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    //private TestableClientTransaction _testableClientTransaction;
    //private TestDataContainerObjectMother _testDataContainerObjectMother;
    private ClientTransaction _testableClientTransaction;
    //private DataContainerObjectMother _testDataContainerObjectMother;
    private TransactionScope _transactionScope;

    // construction and disposing

    protected TestBase ()
    {
    }

    // methods and properties

    [SetUp]
    public virtual void SetUp ()
    {
      //base.SetUp();

      ReInitializeTransaction();
      _transactionScope = new TransactionScope();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      //base.TearDown();
      //_testDataContainerObjectMother = null;
      DisposeTransaction();
      _transactionScope.Dispose();
    }

    protected ClientTransaction TestableClientTransaction
    {
      get { return _testableClientTransaction; }
    }

    //protected TestDataContainerObjectMother TestDataContainerObjectMother
    //{
    //  get { return _testDataContainerObjectMother; }
    //}

    private void DisposeTransaction ()
    {
      ClientTransactionScope.ResetActiveScope();
    }

    protected void ReInitializeTransaction ()
    {
      DisposeTransaction();

      _testableClientTransaction = ClientTransaction.CreateRootTransaction();
      _testableClientTransaction.EnterDiscardingScope();
      //_testDataContainerObjectMother = new TestDataContainerObjectMother ();
    }

    //protected void CheckIfObjectIsDeleted (ObjectID id)
    //{
    //  try
    //  {
    //    DomainObject domainObject = id.GetObject<TestDomainBase> (includeDeleted: true);
    //    Assert.IsNull (domainObject, string.Format ("Object '{0}' was not deleted.", id));
    //  }
    //  catch (ObjectsNotFoundException)
    //  {
    //  }
    //}
  }
}