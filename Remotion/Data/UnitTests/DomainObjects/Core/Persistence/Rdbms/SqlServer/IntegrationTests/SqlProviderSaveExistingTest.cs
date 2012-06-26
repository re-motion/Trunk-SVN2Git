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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderSaveExistingTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void Save ()
    {
      var savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value1,
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty", ClassWithAllDataTypes.EnumType.Value2);

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value2,
            GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (false, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty"));
      Assert.AreEqual (85, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty"));
      Assert.AreEqual (
          new DateTime (2005, 1, 1), GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty"));
      Assert.AreEqual (
          new DateTime (2005, 1, 1, 17, 0, 0),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty"));
      Assert.AreEqual (
          123456.789, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty"));
      Assert.AreEqual (
          987654.321, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value1,
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
      Assert.AreEqual (
          Color.Values.Red(),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty"));
      Assert.AreEqual (
          new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty"));
      Assert.AreEqual (32767, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property"));
      Assert.AreEqual (2147483647, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property"));
      Assert.AreEqual (
          9223372036854775807, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property"));
      Assert.AreEqual (6789.321f, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty"));
      Assert.AreEqual (
          "abcdeföäü", GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty"));
      Assert.AreEqual (
          "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"));
      ResourceManager.IsEqualToImage1 (
          (byte[]) GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty"));

      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty", true);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty", (byte) 42);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty", new DateTime (1972, 10, 26));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty", new DateTime (
          1974, 10, 26, 15, 17, 19));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty", (decimal) 564.956);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty", 5334.2456);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty", ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Green());
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty", new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property", (short) 67);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property", 42424242);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property", 424242424242424242);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty", (float) 42.42);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty", "zyxwvuZaphodBeeblebrox");
      SetPropertyValue (
          savedClassWithAllDataTypes,
          typeof (ClassWithAllDataTypes),
          "StringPropertyWithoutMaxLength",
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876");
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty", ResourceManager.GetImage2());

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty"));
      Assert.AreEqual (42, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty"));
      Assert.AreEqual (
          new DateTime (1972, 10, 26),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty"));
      Assert.AreEqual (
          new DateTime (1974, 10, 26, 15, 17, 19),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty"));
      Assert.AreEqual (564.956, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty"));
      Assert.AreEqual (
          5334.2456, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
      Assert.AreEqual (
          Color.Values.Green(),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty"));
      Assert.AreEqual (
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty"));
      Assert.AreEqual (67, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property"));
      Assert.AreEqual (42424242, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property"));
      Assert.AreEqual (
          424242424242424242, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property"));
      Assert.AreEqual (42.42f, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty"));
      Assert.AreEqual (
          "zyxwvuZaphodBeeblebrox",
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty"));
      Assert.AreEqual (
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876",
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"));
      ResourceManager.IsEqualToImage2 (
          (byte[]) GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty"));
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.AreEqual ((byte) 78, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.AreEqual (
          new DateTime (2005, 2, 1), GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.AreEqual (
          new DateTime (2005, 2, 1, 5, 0, 0),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.AreEqual (765.098m, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.AreEqual (
          654321.789d, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value2,
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.AreEqual (
          (short) 12000, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.AreEqual (-2147483647, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.AreEqual (3147483647L, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.AreEqual (12.456F, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty"));
      Assert.IsNull (GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));

      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty", false);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty", (byte) 100);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty", new DateTime (2007, 1, 18));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty", new DateTime (
          2005, 1, 18, 10, 10, 10));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty", 20.123m);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty", 56.87d);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty", ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty", new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"));
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property", (short) -43);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property", -42);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property", -41L);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty", -40F);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty", ResourceManager.GetImage1());

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (false, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.AreEqual ((byte) 100, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.AreEqual (
          new DateTime (2007, 1, 18), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.AreEqual (
          new DateTime (2005, 1, 18, 10, 10, 10),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.AreEqual (20.123m, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.AreEqual (56.87d, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.AreEqual (
          new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.AreEqual ((short) -43, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.AreEqual (-42, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.AreEqual (-41L, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.AreEqual (-40F, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty"));
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      DataContainer tempClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);
      SetPropertyValue (tempClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty", ResourceManager.GetImage1());
      Provider.Save (new[] { tempClassWithAllDataTypes });
      
      var savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.AreEqual ((byte) 78, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.AreEqual (
          new DateTime (2005, 2, 1), GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.AreEqual (
          new DateTime (2005, 2, 1, 5, 0, 0),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.AreEqual (765.098m, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.AreEqual (
          654321.789d, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value2,
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.AreEqual (
          (short) 12000, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.AreEqual (
          -2147483647, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.AreEqual (
          3147483647L, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.AreEqual (12.456F, GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty"));
      ResourceManager.IsEqualToImage1 (
          (byte[]) GetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));

      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty", null);
      SetPropertyValue (savedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty", null);

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));
    }

    [Test]
    public void SaveWithNoChanges ()
    {
      DataContainer classWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Provider.Save (new[] { classWithAllDataTypes });

      // expectation: no exception
    }


    [Test]
    public void SaveMultipleDataContainers ()
    {
      DataContainer savedOrderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer savedOrderItemContainer = LoadDataContainer (DomainObjectIDs.OrderItem1);

      Assert.AreEqual (1, GetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber"));
      Assert.AreEqual ("Mainboard", GetPropertyValue (savedOrderItemContainer, typeof (OrderItem), "Product"));

      SetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber", 10);
      SetPropertyValue (savedOrderItemContainer, typeof (OrderItem), "Product", "Raumschiff");

      Provider.Save (new[] { savedOrderContainer, savedOrderItemContainer });

      DataContainer orderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderItemContainer = ReloadDataContainer (DomainObjectIDs.OrderItem1);

      Assert.AreEqual (10, GetPropertyValue (orderContainer, typeof (Order), "OrderNumber"));
      Assert.AreEqual ("Raumschiff", GetPropertyValue (orderItemContainer, typeof (OrderItem), "Product"));
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage =
        "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else."
        )]
    public void ConcurrentSave ()
    {
      DataContainer orderContainer1 = LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderContainer2 = LoadDataContainer (DomainObjectIDs.Order1);

      SetPropertyValue (orderContainer1, typeof (Order), "OrderNumber", 10);
      SetPropertyValue (orderContainer2, typeof (Order), "OrderNumber", 11);

      Provider.Save (new[] { orderContainer1 });
      Provider.Save (new[] { orderContainer2 });
    }

    [Test]
    public void SaveWithConnect ()
    {
      DataContainer savedOrderContainer = LoadDataContainerWithSeparateProvider (DomainObjectIDs.Order1);
      SetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber", 10);

      Assert.That (!Provider.IsConnected);

      Provider.Save (new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, GetPropertyValue (reloadedOrderContainer, typeof (Order), "OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void WrapSqlException ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      SetPropertyValue (orderContainer, typeof (Order), "Customer", new ObjectID (typeof (Customer), Guid.NewGuid()));

      Provider.Save (new[] { orderContainer });
    }

    [Test]
    public void UpdateTimestamps ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      object oldTimestamp = orderContainer.Timestamp;
      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { orderContainer });
      Provider.UpdateTimestamps (new[] { orderContainer });

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }

    [Test]
    public void UpdateTimestampsWithConnect ()
    {
      DataContainer orderContainer = LoadDataContainerWithSeparateProvider (DomainObjectIDs.Order1);
      object oldTimestamp = orderContainer.Timestamp;
      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 10);

      Assert.That (!Provider.IsConnected);

      Provider.UpdateTimestamps (new[] { orderContainer });

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }

    [Test]
    public void UpdateTimestampsForMultipleDataContainers ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderItemContainer = LoadDataContainer (DomainObjectIDs.OrderItem1);

      object oldOrderTimestamp = orderContainer.Timestamp;
      object oldOrderItemTimestamp = orderItemContainer.Timestamp;

      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 10);
      SetPropertyValue (orderItemContainer, typeof (OrderItem), "Product", "Raumschiff");

      Provider.Save (new[] { orderContainer, orderItemContainer });
      Provider.UpdateTimestamps (new[] { orderContainer, orderItemContainer });

      Assert.IsFalse (oldOrderTimestamp.Equals (orderContainer.Timestamp));
      Assert.IsFalse (oldOrderItemTimestamp.Equals (orderItemContainer.Timestamp));
    }

    [Test]
    public void TransactionalSave ()
    {
      DataContainer savedOrderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      SetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber", 10);

      Provider.BeginTransaction();
      Provider.Save (new[] { savedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, GetPropertyValue (reloadedOrderContainer, typeof (Order), "OrderNumber"));
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      Provider.BeginTransaction ();
      DataContainer reloadedAndSavedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      SetPropertyValue (reloadedAndSavedOrderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { reloadedAndSavedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, GetPropertyValue (reloadedOrderContainer, typeof (Order), "OrderNumber"));
    }

    [Test]
    public void TransactionalLoadDataContainersByRelatedIDAndSave ()
    {
      Provider.BeginTransaction();

      var relationEndPointDefinition = GetEndPointDefinition (typeof (OrderTicket), "Order");
      var orderTicketContainers = Provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Order1).ToList();

      SetPropertyValue (orderTicketContainers[0], typeof (OrderTicket), "FileName", "C:\newFile.jpg");

      Provider.Save (orderTicketContainers);
      Provider.Commit();

      DataContainer reloadedOrderTicketContainer = ReloadDataContainer (DomainObjectIDs.OrderTicket1);
      Assert.AreEqual ("C:\newFile.jpg", GetPropertyValue (reloadedOrderTicketContainer, typeof (OrderTicket), "FileName"));
    }

    [Test]
    public void TransactionalSaveAndSetTimestamp ()
    {
      Provider.BeginTransaction();
      DataContainer savedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      object oldTimestamp = savedOrderContainer.Timestamp;
      SetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { savedOrderContainer });
      Provider.UpdateTimestamps (new[] { savedOrderContainer });
      Provider.Commit ();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, GetPropertyValue (reloadedOrderContainer, typeof (Order), "OrderNumber"));
      Assert.IsFalse (oldTimestamp.Equals (reloadedOrderContainer.Timestamp));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Commit cannot be called without calling BeginTransaction first.")]
    public void CommitWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { orderContainer });
      Provider.Commit ();
    }

    [Test]
    public void SaveWithRollback ()
    {
      Provider.BeginTransaction ();
      DataContainer savedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      SetPropertyValue (savedOrderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { savedOrderContainer });
      Provider.UpdateTimestamps (new[] { savedOrderContainer });
      Provider.Rollback ();

      DataContainer reloadedOrderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (1, GetPropertyValue (reloadedOrderContainer, typeof (Order), "OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Rollback cannot be called without calling BeginTransaction first.")]
    public void RollbackWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 10);

      Provider.Save (new[] { orderContainer });
      Provider.Rollback ();
    }

    [Test]
    public void SaveForeignKeyInSameStorageProvider ()
    {
      DataContainer orderTicketContainer = LoadDataContainer (DomainObjectIDs.OrderTicket1);
      SetPropertyValue (orderTicketContainer, typeof (OrderTicket), "Order", DomainObjectIDs.Order2);

      Provider.Save (new[] { orderTicketContainer });

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      DataContainer savedOrderContainer = LoadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      SetPropertyValue (savedOrderContainer, typeof (Order), "Official", DomainObjectIDs.Official2);

      Provider.Save (new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      Assert.AreEqual (DomainObjectIDs.Official2, GetPropertyValue (reloadedOrderContainer, typeof (Order), "Official"));
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer (DomainObjectIDs.Ceo1);
      SetPropertyValue (savedCeoContainer, typeof (Ceo), "Company", DomainObjectIDs.Partner1);

      Provider.Save (new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Partner1, GetPropertyValue (reloadedCeoContainer, typeof (Ceo), "Company"));
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer (DomainObjectIDs.Ceo1);
      SetPropertyValue (savedCeoContainer, typeof (Ceo), "Company", DomainObjectIDs.Supplier1);

      Provider.Save (new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Supplier1, GetPropertyValue (reloadedCeoContainer, typeof (Ceo), "Company"));
    }

    [Test]
    public void SaveNullForeignKey ()
    {
      DataContainer savedComputerContainer = LoadDataContainer (DomainObjectIDs.Computer1);
      var propertyDefinition = GetPropertyDefinition (typeof (Computer), "Employee");
      savedComputerContainer.SetValue (propertyDefinition, null);
      Provider.Save (new[] { savedComputerContainer });

      DataContainer reloadedComputerContainer = ReloadDataContainer (DomainObjectIDs.Computer1);
      Assert.IsNull (reloadedComputerContainer.GetValue (propertyDefinition, ValueAccess.Current));
    }

    [Test]
    public void SaveNullForeignKeyWithInheritance ()
    {
      DataContainer savedCeoContainer = LoadDataContainer (DomainObjectIDs.Ceo1);
      SetPropertyValue (savedCeoContainer, typeof (Ceo), "Company", null);

      Provider.Save (new[] { savedCeoContainer });
      Provider.Disconnect();

      using (SqlConnection connection = new SqlConnection (TestDomainConnectionString))
      {
        connection.Open ();
        using (SqlCommand command = new SqlCommand ("select * from Ceo where ID = @id", connection))
        {
          command.Parameters.AddWithValue ("@id", DomainObjectIDs.Ceo1.Value);
          using (SqlDataReader reader = command.ExecuteReader ())
          {
            reader.Read ();
            int columnOrdinal = reader.GetOrdinal ("CompanyIDClassID");
            Assert.IsTrue (reader.IsDBNull (columnOrdinal));
          }
        }
      }
    }

    private DataContainer LoadDataContainer (ObjectID id)
    {
      return Provider.LoadDataContainer (id).LocatedObject;
    }

    private DataContainer LoadDataContainerWithSeparateProvider (ObjectID id)
    {
      using (var separateProvider = CreateRdbmsProvider ())
      {
        return separateProvider.LoadDataContainer (id).LocatedObject;
      }
    }

    private DataContainer ReloadDataContainer (ObjectID id)
    {
      Provider.Disconnect();

      return LoadDataContainerWithSeparateProvider (id);
    }
  }
}