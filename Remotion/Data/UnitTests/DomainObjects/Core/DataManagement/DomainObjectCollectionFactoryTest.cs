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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DomainObjectCollectionFactoryTest : StandardMappingTest
  {
    private DomainObjectCollectionFactory _factory;
    private DomainObjectCollectionData _data;
    private ModificationCheckingCollectionDataDecorator _dataWithOrderType;
    private Order _orderA;
    private Order _orderB;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = DomainObjectCollectionFactory.Instance;
      _data = new DomainObjectCollectionData ();
      _dataWithOrderType = new ModificationCheckingCollectionDataDecorator (typeof (Order), _data);

      _orderA = DomainObjectMother.CreateFakeObject<Order> ();
      _orderB = DomainObjectMother.CreateFakeObject<Order> ();
    }

    [Test]
    public void CreateCollection_SetsData_ObjectListCtor ()
    {
      var collection = _factory.CreateCollection (typeof (ObjectList<Order>), _dataWithOrderType);

      Assert.That (collection, Is.InstanceOfType (typeof (ObjectList<Order>)));
      CheckDataStrategy (collection, _dataWithOrderType);
    }

    [Test]
    public void CreateCollection_SetsData_DomainObjectCollectionCtor ()
    {
      var collection = _factory.CreateCollection (typeof (DomainObjectCollection), _data);

      Assert.That (collection, Is.InstanceOfType (typeof (DomainObjectCollection)));
      CheckDataStrategy (collection, _data);
    }

    [Test]
    public void CreateCollection_ProtectedCtor ()
    {
      var collection = _factory.CreateCollection (typeof (CollectionWithProtectedCtor), _dataWithOrderType);

      Assert.That (collection, Is.InstanceOfType (typeof (CollectionWithProtectedCtor)));
      CheckDataStrategy (collection, _dataWithOrderType);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = 
        "Cannot create an instance of 'Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain.CollectionWithMissingDataCtor' because "
        + "that type does not provide a constructor taking an IDomainObjectCollectionData object.", 
        MatchType = MessageMatch.Contains)]
    public void CreateCollection_ThrowsIfNoSupportedCtor ()
    {
      _factory.CreateCollection (typeof (CollectionWithMissingDataCtor), _data);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void CreateCollection_ThrowsIfNonCollectionType ()
    {
      _factory.CreateCollection (typeof (CollectionNotDerivedFromDomainObjectCollection), _data);
    }

    [Test]
    public void CreateCollection_ForStandaloneCollection ()
    {
      DomainObjectCollection collection = _factory.CreateCollection (typeof (ObjectList<Order>), new[] { _orderA, _orderB }, typeof (Order));

      Assert.That (collection, Is.Not.Null);
      Assert.That (collection.RequiredItemType, Is.EqualTo (typeof (Order)));

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (collection, typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void CreateCollection_ForStandaloneCollection_PerformsItemChecks ()
    {
      DomainObjectCollection collection = _factory.CreateCollection (typeof (ObjectList<Order>), new Order[] { null }, typeof (Order));

      Assert.That (collection, Is.Not.Null);
      Assert.That (collection.RequiredItemType, Is.EqualTo (typeof (Order)));

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (collection, typeof (Order));
    }

    [Test]
    public void CreateCollection_ForStandaloneCollection_WithInferredItemType ()
    {
      DomainObjectCollection collection = _factory.CreateCollection (typeof (ObjectList<Order>), new Order[0]);

      Assert.That (collection.RequiredItemType, Is.SameAs (typeof (Order)));
    }

    [Test]
    public void CreateCollection_ForStandaloneCollection_WithInferredItemType_NoneFound ()
    {
      DomainObjectCollection collection = _factory.CreateCollection (typeof (DomainObjectCollection), new Order[0]);

      Assert.That (collection.RequiredItemType, Is.Null);
    }

    [Test]
    public void CreateReadOnlyCollection ()
    {
      var collection = _factory.CreateReadOnlyCollection (typeof (OrderCollection), new Order[0]);

      Assert.That (collection, Is.TypeOf (typeof (OrderCollection)));
      Assert.That (collection.IsReadOnly, Is.True);
      Assert.That (collection, Is.Empty);
    }

    [Test]
    public void CreateReadOnlyCollection_Contents ()
    {
      var collection = _factory.CreateReadOnlyCollection (typeof (OrderCollection), new[] { _orderA, _orderB });

      Assert.That (collection, Is.EqualTo (new[] { _orderA, _orderB }));
    }

    [Test]
    public void CreateReadOnlyCollection_DataStrategy ()
    {
      var collection = _factory.CreateReadOnlyCollection (typeof (OrderCollection), new Order[0]);

      DomainObjectCollectionDataTestHelper.CheckReadOnlyCollectionStrategy (collection, true);
    }
    
    private void CheckDataStrategy (DomainObjectCollection collection, IDomainObjectCollectionData expectedData)
    {
      var data = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (collection);
      Assert.That (data, Is.SameAs (expectedData));
    }
  }
}