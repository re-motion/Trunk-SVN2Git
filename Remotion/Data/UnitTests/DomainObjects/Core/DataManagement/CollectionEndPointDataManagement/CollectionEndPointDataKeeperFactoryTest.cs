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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class CollectionEndPointDataKeeperFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IRelationEndPointProvider _relationEndPointProvider;

    private CollectionEndPointDataKeeperFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _relationEndPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _factory = new CollectionEndPointDataKeeperFactory (_clientTransaction, _relationEndPointProvider);
    }

    [Test]
    public void Create ()
    {
      var relationEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Customer1, 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      var comparer = Comparer<DomainObject>.Default;

      var result = _factory.Create (relationEndPointID, comparer);

      Assert.That (result, Is.TypeOf (typeof (CollectionEndPointDataKeeper)));
      Assert.That (((CollectionEndPointDataKeeper) result).EndPointID, Is.SameAs(relationEndPointID));
      Assert.That (((CollectionEndPointDataKeeper) result).SortExpressionBasedComparer, Is.SameAs (comparer));
    }

    [Test]
    public void Create_WithNullComparer ()
    {
      var relationEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Customer1,
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");

      var result = _factory.Create (relationEndPointID, null);

      Assert.That (result, Is.TypeOf (typeof (CollectionEndPointDataKeeper)));
      Assert.That (((CollectionEndPointDataKeeper) result).EndPointID, Is.SameAs (relationEndPointID));
      Assert.That (((CollectionEndPointDataKeeper) result).SortExpressionBasedComparer, Is.Null);
    }

    [Test]
    public void Serializable ()
    {
      var serializableProvider = new SerializableRelationEndPointProviderFake();
      var factory = new CollectionEndPointDataKeeperFactory (_clientTransaction, serializableProvider);

      var deserializedInstance = Serializer.SerializeAndDeserialize (factory);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointProvider, Is.Not.Null);
    }
  }
}