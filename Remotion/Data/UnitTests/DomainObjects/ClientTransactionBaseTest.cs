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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects
{
  public class ClientTransactionBaseTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClientTransactionMock _clientTransactionMock;
    private ClientTransactionScope _transactionScope;
    private TestDataContainerObjectMother _testDataContainerObjectMother;

    // construction and disposing

    protected ClientTransactionBaseTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();

      ReInitializeTransaction();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _testDataContainerObjectMother = null;
      DisposeTransaction();
    }

    protected ClientTransactionMock ClientTransactionMock
    {
      get { return _clientTransactionMock; }
    }

    protected TestDataContainerObjectMother TestDataContainerObjectMother
    {
      get { return _testDataContainerObjectMother; }
    }

    private void DisposeTransaction ()
    {
      if (_transactionScope != null)
      {
        if (ClientTransactionScope.ActiveScope == _transactionScope)
          _transactionScope.Leave();
        else
          ClientTransactionScope.ResetActiveScope();
        _transactionScope = null;
        _clientTransactionMock = null;
      }
    }

    protected void ReInitializeTransaction ()
    {
      DisposeTransaction();

      _clientTransactionMock = new ClientTransactionMock();
      _transactionScope = _clientTransactionMock.EnterDiscardingScope();
      _testDataContainerObjectMother = new TestDataContainerObjectMother ();
    }

    protected void CheckIfObjectIsDeleted (ObjectID id)
    {
      try
      {
        DomainObject domainObject = TestDomainBase.GetObject (id, true);
        Assert.IsNull (domainObject, string.Format ("Object '{0}' was not deleted.", id));
      }
      catch (ObjectNotFoundException)
      {
      }
    }
  }
}
