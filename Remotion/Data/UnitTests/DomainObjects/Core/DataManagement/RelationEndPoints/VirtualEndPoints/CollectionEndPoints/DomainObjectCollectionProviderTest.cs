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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionProviderTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private DomainObjectCollectionProvider _provider;

    private IDomainObjectCollectionData _dataStrategyStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      _provider = new DomainObjectCollectionProvider();

      _dataStrategyStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _dataStrategyStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
    }

    [Test]
    public void GetCollection_Once ()
    {
      var result = _provider.GetCollection (_endPointID, () => _dataStrategyStub);

      Assert.That (result, Is.TypeOf<OrderCollection>());
      Assert.That (
          DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (result), 
          Is.SameAs (_dataStrategyStub));
    }

    [Test]
    public void GetCollection_Twice ()
    {
      var result1 = _provider.GetCollection (_endPointID, () => _dataStrategyStub);
      var result2 = _provider.GetCollection (_endPointID, () => { throw new Exception ("Must not be called."); });

      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var result = _provider.GetCollectionWithOriginalData (_endPointID, () => _dataStrategyStub);

      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (
          DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (result),
          Is.SameAs (_dataStrategyStub));
    }

    [Test]
    public void GetCollectionWithOriginalData_Twice ()
    {
      var result1 = _provider.GetCollectionWithOriginalData (_endPointID, () => _dataStrategyStub);
      var result2 = _provider.GetCollectionWithOriginalData (_endPointID, () => _dataStrategyStub);

      Assert.That (result2, Is.Not.SameAs (result1));
    }

    [Test]
    public void RegisterCollection ()
    {
      var collection = new DomainObjectCollection (_dataStrategyStub);

      _provider.RegisterCollection (_endPointID, collection);

      var registered = _provider.GetCollection (_endPointID, () => { throw new Exception ("Must not be called."); });
      Assert.That (registered, Is.SameAs (collection));
    }

    [Test]
    public void RegisterCollection_Twice ()
    {
      var collection1 = new DomainObjectCollection (_dataStrategyStub);
      _provider.RegisterCollection (_endPointID, collection1);

      var collection2 = new DomainObjectCollection (_dataStrategyStub);
      _provider.RegisterCollection (_endPointID, collection2);
      
      var registered = _provider.GetCollection (_endPointID, () => { throw new Exception ("Must not be called."); });
      Assert.That (registered, Is.SameAs (collection2));
    }

    [Test]
    public void RegisterCollection_AfterGetCollection ()
    {
      var collection1 = _provider.GetCollection (_endPointID, () => _dataStrategyStub);
      var registered1 = _provider.GetCollection (_endPointID, () => { throw new Exception ("Must not be called."); });
      Assert.That (registered1, Is.SameAs (collection1));

      var collection2 = new DomainObjectCollection (_dataStrategyStub);
      _provider.RegisterCollection (_endPointID, collection2);

      var registered2 = _provider.GetCollection (_endPointID, () => { throw new Exception ("Must not be called."); });
      Assert.That (registered2, Is.SameAs (collection2));
    }

  }
}