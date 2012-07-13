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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class FetchEnabledObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IFetchEnabledPersistenceStrategy _persistenceStrategyMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;
    private IDataContainerLifetimeManager _lifetimeManagerStub;

    private FetchEnabledObjectLoader _fetchEnabledObjectLoader;
    
    private IQuery _fakeQuery;

    private LoadedObjectDataWithDataSourceData _resultItem1;
    private LoadedObjectDataWithDataSourceData _resultItem2;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IFetchEnabledPersistenceStrategy> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent> ();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider> ();
      _lifetimeManagerStub = _mockRepository.StrictMock<IDataContainerLifetimeManager> ();

      _fetchEnabledObjectLoader = new FetchEnabledObjectLoader (
          _persistenceStrategyMock,
          _loadedObjectDataRegistrationAgentMock,
          _lifetimeManagerStub,
          _loadedObjectDataProviderStub);

      _fakeQuery = CreateFakeQuery ();

      _resultItem1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order1);
      _resultItem2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order2);
    }

    [Test]
    public void GetOrLoadFetchQueryResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteFetchQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _resultItem1, _resultItem2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _resultItem1.LoadedObjectData, _resultItem2.LoadedObjectData }),
              Arg.Is (_lifetimeManagerStub), 
              Arg.Is (true)));

      _mockRepository.ReplayAll ();

      var result = _fetchEnabledObjectLoader.GetOrLoadFetchQueryResult (_fakeQuery);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _resultItem1, _resultItem2 }));
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "TEST",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }
  }
}