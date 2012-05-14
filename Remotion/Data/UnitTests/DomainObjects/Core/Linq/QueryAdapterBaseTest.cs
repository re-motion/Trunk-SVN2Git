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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class QueryAdapterBaseTest : StandardMappingTest
  {
    private IQuery _queryStub;
    private QueryAdapterBase<object> _queryAdapterBase;

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();

      _queryStub = MockRepository.GenerateStub<IQuery>();

      _queryAdapterBase = MockRepository.GeneratePartialMock<QueryAdapterBase<object>>(_queryStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_queryAdapterBase.Query, Is.SameAs (_queryStub));
    }

    [Test]
    public void ID ()
    {
      _queryStub.Stub (stub => stub.ID).Return ("ID");

      var result = _queryAdapterBase.ID;

      Assert.That (result, Is.EqualTo ("ID"));
    }

    [Test]
    public void Statement ()
    {
      _queryStub.Stub (stub => stub.Statement).Return ("Teststatment");

      var result = _queryAdapterBase.Statement;

      Assert.That (result, Is.EqualTo ("Teststatment"));
    }

    [Test]
    public void StorageProviderDefinition ()
    {
      _queryStub.Stub (stub => stub.StorageProviderDefinition).Return (UnitTestStorageProviderDefinition);

      var result = _queryAdapterBase.StorageProviderDefinition;

      Assert.That (result, Is.SameAs (UnitTestStorageProviderDefinition));
    }

    [Test]
    public void CollectionType ()
    {
      _queryStub.Stub (stub => stub.CollectionType).Return (typeof(object));

      var result = _queryAdapterBase.CollectionType;

      Assert.That (result, Is.SameAs (typeof(object)));
    }

    [Test]
    public void QueryType ()
    {
      _queryStub.Stub (stub => stub.QueryType).Return (Data.DomainObjects.Queries.Configuration.QueryType.Collection);

      var result = _queryAdapterBase.QueryType;

      Assert.That (result, Is.EqualTo (Data.DomainObjects.Queries.Configuration.QueryType.Collection));
    }

    [Test]
    public void Parameters ()
    {
      var fakeResult = new QueryParameterCollection();

      _queryStub.Stub (stub => stub.Parameters).Return (fakeResult);

      var result = _queryAdapterBase.Parameters;

      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void EagerFetchQueries ()
    {
      var fakeResult = new EagerFetchQueryCollection ();

      _queryStub.Stub (stub => stub.EagerFetchQueries).Return (fakeResult);

      var result = _queryAdapterBase.EagerFetchQueries;

      Assert.That (result, Is.SameAs (fakeResult));
    }
    
  }
}