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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class SubQueryManagerTest
  {
    private SubQueryManager _queryManager;
    private ClientTransaction _parentTransactionStub;
    private IQueryManager _parentQueryManagerMock;
    private SubClientTransaction _subTransactionMock;
    private IQuery _queryMock;

    [SetUp]
    public void SetUp()
    {
      _parentTransactionStub = MockRepository.GenerateStub<ClientTransaction> (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection());
      _parentQueryManagerMock = MockRepository.GenerateMock<IQueryManager> ();
      _parentTransactionStub.Stub (stub => _parentTransactionStub.QueryManager).Return (_parentQueryManagerMock);
      _subTransactionMock = MockRepository.GenerateMock<SubClientTransaction> (_parentTransactionStub);
      _subTransactionMock.Stub (mock => mock.ParentTransaction).Return (_parentTransactionStub);
      _queryManager = new SubQueryManager (_subTransactionMock);
      _queryMock = MockRepository.GenerateMock<IQuery> ();
    }

    [Test]
    public void GetScalar_DelegatedToParent()
    {
      _parentQueryManagerMock
          .Expect (mock => mock.GetScalar (_queryMock))
          .Do (invocation => Assert.That (_parentTransactionStub.IsReadOnly, Is.True))
          .Return (7);
      _parentQueryManagerMock.Replay ();

      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      var result = _queryManager.GetScalar (_queryMock);

      _parentQueryManagerMock.VerifyAllExpectations ();
      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      Assert.That (result, Is.EqualTo (7));
    }

    [Test]
    public void GetCollection_NonGeneric_DelegatedToParent ()
    {
      var expectedResult = new DomainObjectCollection ();

      _parentQueryManagerMock
          .Expect (mock => mock.GetCollection (_queryMock))
          .Do (invocation => Assert.That (_parentTransactionStub.IsReadOnly, Is.False))
          .Return (expectedResult);
      _parentQueryManagerMock.Replay ();

      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      var result = _queryManager.GetCollection (_queryMock);

      _parentQueryManagerMock.VerifyAllExpectations ();
      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      Assert.That (result, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCollection_Generic_DelegatedToParent ()
    {
      var expectedResult = new ObjectList<Order> ();

      _parentQueryManagerMock
          .Expect (mock => mock.GetCollection<Order> (_queryMock))
          .Do (invocation => Assert.That (_parentTransactionStub.IsReadOnly, Is.False))
          .Return (expectedResult);
      _parentQueryManagerMock.Replay ();

      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      var result = _queryManager.GetCollection<Order> (_queryMock);

      _parentQueryManagerMock.VerifyAllExpectations ();
      Assert.That (_parentTransactionStub.IsReadOnly, Is.True);
      Assert.That (result, Is.EqualTo (expectedResult));
    }
 }
}
