// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class EagerFetcherTest : ClientTransactionBaseTest
  {
    private IQueryManager _queryManagerMock;
    private IQuery _testQuery;
    private IQuery _fetchTestQuery;
    private IRelationEndPointDefinition _officialOrdersRelationEndPointDefinition;
    private IRelationEndPointDefinition _objectEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      _queryManagerMock = MockRepository.GenerateMock<IQueryManager> ();

      var storageProviderID = DomainObjectIDs.Official1.StorageProviderID;
      _testQuery = QueryFactory.CreateCollectionQuery ("test query", storageProviderID, "TEST QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _fetchTestQuery = QueryFactory.CreateCollectionQuery ("fetch query", storageProviderID, "FETCH QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _officialOrdersRelationEndPointDefinition = DomainObjectIDs.Official1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Official).FullName + ".Orders");
      _objectEndPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
    }

    [Test]
    public void FetchRelatedObjects_ExecutesFetchQuery ()
    {
      _queryManagerMock.Expect (mock => mock.GetCollection (_fetchTestQuery)).Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[0]));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[0]);
      fetcher.FetchRelatedObjects (_officialOrdersRelationEndPointDefinition, _fetchTestQuery);

      _queryManagerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Eager fetching is only supported for collection-valued relation properties.\r\n"
        + "Parameter name: relationEndPointDefinition")]
    public void FetchRelatedObjects_ObjectEndPoint ()
    {
      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[0]);
      fetcher.FetchRelatedObjects (_objectEndPointDefinition, _fetchTestQuery);
    }

  }
}