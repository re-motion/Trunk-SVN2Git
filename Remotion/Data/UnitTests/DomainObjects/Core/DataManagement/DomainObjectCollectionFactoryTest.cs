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

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DomainObjectCollectionFactoryTest
  {
    private DomainObjectCollectionFactory _factory;
    private DomainObjectCollectionData _data;

    [SetUp]
    public void SetUp ()
    {
      _factory = new DomainObjectCollectionFactory ();
      _data = new DomainObjectCollectionData ();
    }

    [Test]
    public void CreateCollection_SetsData_ObjectListCtor ()
    {
      var collection = _factory.CreateCollection (typeof (ObjectList<Order>), _data, null);

      Assert.That (collection, Is.InstanceOfType (typeof (ObjectList<Order>)));
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<DomainObjectCollectionData> (decorator);
      Assert.That (wrappedData, Is.SameAs (_data));
    }

    [Test]
    public void CreateCollection_SetsData_DomainObjectCollectionCtor ()
    {
      var collection = _factory.CreateCollection (typeof (DomainObjectCollection), _data, null);

      Assert.That (collection, Is.InstanceOfType (typeof (DomainObjectCollection)));
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<DomainObjectCollectionData> (decorator);
      Assert.That (wrappedData, Is.SameAs (_data));
    }

    [Test]
    public void CreateCollection_ProtectedCtor ()
    {
      var collection = _factory.CreateCollection (typeof (CollectionWithProtectedCtor), _data, null);

      Assert.That (collection, Is.InstanceOfType (typeof (CollectionWithProtectedCtor)));
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<DomainObjectCollectionData> (decorator);
      Assert.That (wrappedData, Is.SameAs (_data));
    }

    [Test]
    public void CreateCollection_PassesRequiredItemType ()
    {
      var collection1 = _factory.CreateCollection (typeof (DomainObjectCollection), _data, null);
      var collection2 = _factory.CreateCollection (typeof (DomainObjectCollection), _data, typeof (Order));

      Assert.That (collection1.RequiredItemType, Is.Null);
      Assert.That (collection2.RequiredItemType, Is.SameAs (typeof (Order)));
    }

    [Test]
    public void CreateCollection_ChecksRequiredItemType_IfProvided ()
    {
      _factory.CreateCollection (typeof (OrderCollection), _data, typeof (Order));
      _factory.CreateCollection (typeof (OrderCollection), _data, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot create an instance of 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection' with required item type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer': The collection's constructor sets a different required item type.")]
    public void CreateCollection_ChecksRequiredItemType_ThrowsIfInvalidRequiredItemType ()
    {
      _factory.CreateCollection (typeof (OrderCollection), _data, typeof (Customer));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = 
        "Cannot create an instance of 'Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain.CollectionWithMissingDataCtor' because "
        + "that type does not provide a constructor taking an IDomainObjectCollectionData object and optionally a required item type.", 
        MatchType = MessageMatch.Contains)]
    public void CreateCollection_ThrowsIfNoSupportedCtor ()
    {
      _factory.CreateCollection (typeof (CollectionWithMissingDataCtor), _data, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void CreateCollection_ThrowsIfNonCollectionType ()
    {
      _factory.CreateCollection (typeof (CollectionNotDerivedFromDomainObjectCollection), _data, null);
    }
  }
}