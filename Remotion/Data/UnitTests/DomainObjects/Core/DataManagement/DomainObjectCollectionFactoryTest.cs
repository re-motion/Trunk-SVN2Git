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
  public class DomainObjectCollectionFactoryTest : ClientTransactionBaseTest
  {
    private DomainObjectCollectionFactory _factory;
    private DomainObjectCollectionData _data;
    private ArgumentCheckingCollectionDataDecorator _dataWithOrderType;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new DomainObjectCollectionFactory ();
      _data = new DomainObjectCollectionData ();
      _dataWithOrderType = new ArgumentCheckingCollectionDataDecorator (_data, typeof (Order));
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
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);
      
      DomainObjectCollection collection = _factory.CreateCollection (typeof (ObjectList<Order>), new[] { order1, order2 }, typeof (Order));

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

    private void CheckDataStrategy (DomainObjectCollection collection, IDomainObjectCollectionData expectedData)
    {
      var data = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (collection);
      Assert.That (data, Is.SameAs (expectedData));
    }
  }
}