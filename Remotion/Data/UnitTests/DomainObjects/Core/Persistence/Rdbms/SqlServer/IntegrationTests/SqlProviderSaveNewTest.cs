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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderSaveNewTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void NewDataContainer ()
    {
      DataContainer newDataContainer = CreateNewDataContainer (typeof (Computer));
      SetPropertyValue (newDataContainer, typeof (Computer), "SerialNumber", "123");

      ObjectID newID = newDataContainer.ID;

      Provider.Save (new[] { newDataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer (newID);

      Assert.IsNotNull (reloadedDataContainer);
      Assert.AreEqual (newDataContainer.ID, reloadedDataContainer.ID);
    }

    [Test]
    public void AllDataTypes ()
    {
      DataContainer classWithAllDataTypes = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = classWithAllDataTypes.ID;

      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty", true);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty", (byte) 42);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty", new DateTime (1974, 10, 25));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty", new DateTime (
          1974, 10, 26, 18, 9, 18));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty", (decimal) 564.956);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty", 5334.2456);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty",
          ClassWithAllDataTypes.EnumType.Value0);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty", Color.Values.Green());
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty",
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property", (short) 67);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property", 42424242);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property", 424242424242424242);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty", (float) 42.42);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty", "zyxwvuZaphodBeeblebrox");
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength",
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876");
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty", ResourceManager.GetImage1());

      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty", false);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty", (byte) 21);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty", new DateTime (2007, 1, 18));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty", new DateTime (
          2005, 1, 18, 11, 11, 11));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty", 50m);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty", 56.87d);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty",
          ClassWithAllDataTypes.EnumType.Value1);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty",
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property", (short) 51);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property", 52);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property", 53L);
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty", 54F);

      Provider.Save (new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer (newID);

      Assert.AreEqual (true, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty"));
      Assert.AreEqual (42, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty"));
      Assert.AreEqual (
          new DateTime (1974, 10, 25), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty"));
      Assert.AreEqual (
          new DateTime (1974, 10, 26, 18, 9, 18),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty"));
      Assert.AreEqual (564.956, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty"));
      Assert.AreEqual (5334.2456d, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
      Assert.AreEqual (
          Color.Values.Green(), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty"));
      Assert.AreEqual (
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty"));
      Assert.AreEqual (67, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property"));
      Assert.AreEqual (42424242, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property"));
      Assert.AreEqual (
          424242424242424242, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property"));
      Assert.AreEqual (42.42f, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty"));
      Assert.AreEqual (
          "zyxwvuZaphodBeeblebrox", GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty"));
      Assert.AreEqual (
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876",
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"));
      ResourceManager.IsEqualToImage1 (
          (byte[]) GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty"));

      Assert.AreEqual (false, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.AreEqual (21, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.AreEqual (
          new DateTime (2007, 1, 18), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.AreEqual (
          new DateTime (2005, 1, 18, 11, 11, 11),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.AreEqual (50m, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.AreEqual (56.87d, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value1,
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.AreEqual (51, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.AreEqual (52, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.AreEqual (53, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.AreEqual (54F, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleProperty"));

      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringWithNullValueProperty"));
      Assert.IsNull (
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));
    }

    [Test]
    public void AllDataTypes_DefaultValues ()
    {
      DataContainer classWithAllDataTypes = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = classWithAllDataTypes.ID;

      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty", new DateTime (1753, 1, 1));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty", new DateTime (
          1753, 1, 1, 0, 0, 0));
      SetPropertyValue (classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DecimalProperty", 0m);

      Provider.Save (new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer (newID);

      Assert.AreEqual (false, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BooleanProperty"));
      Assert.AreEqual ((byte) 0, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty"));
      Assert.AreEqual (
          new DateTime (1753, 1, 1), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty"));
      Assert.AreEqual (
          new DateTime (1753, 1, 1, 0, 0, 0),
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateTimeProperty"));
      Assert.AreEqual (0d, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "DoubleProperty"));
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "EnumProperty"));
      Assert.AreEqual (
          Color.Values.Blue(), GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty"));
      Assert.AreEqual (Guid.Empty, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty"));
      Assert.AreEqual ((short) 0, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property"));
      Assert.AreEqual (0, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property"));
      Assert.AreEqual (0L, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property"));
      Assert.AreEqual (0F, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty"));
      Assert.AreEqual (string.Empty, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringProperty"));
      Assert.AreEqual (
          string.Empty, GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"));
      ResourceManager.IsEmptyImage (
          (byte[]) GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "BinaryProperty"));

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

      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaBooleanWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDecimalWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaDoubleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt16WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt32WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaInt64WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaSingleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "StringWithNullValueProperty"));
      Assert.IsNull (
          GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (reloadedClassWithAllDataTypes, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));
    }

    [Test]
    public void ExistingObjectRelatesToNew ()
    {
      var newDataContainer = CreateNewDataContainer (typeof (Employee));
      var existingDataContainer = LoadDataContainer (DomainObjectIDs.Employee1);

      newDataContainer.SetValue (GetPropertyDefinition (typeof (Employee), "Name"), "Supervisor");
      var supervisorProperty = GetPropertyDefinition (typeof (Employee), "Supervisor");
      existingDataContainer.SetValue (supervisorProperty, newDataContainer.ID);

      Provider.Save (new[] { newDataContainer, existingDataContainer });

      DataContainer newSupervisorContainer = ReloadDataContainer (newDataContainer.ID);
      DataContainer existingSubordinateContainer = ReloadDataContainer (existingDataContainer.ID);

      Assert.IsNotNull (newSupervisorContainer);
      Assert.AreEqual (newSupervisorContainer.ID, existingSubordinateContainer.GetValue (supervisorProperty));
    }

    [Test]
    public void NewObjectRelatesToExisting ()
    {
      var newDataContainer = CreateNewDataContainer (typeof (Order));
      var deliveryDateProperty = GetPropertyDefinition (typeof (Order), "DeliveryDate");
      var customerProperty = GetPropertyDefinition (typeof (Order), "Customer");
      var officialProperty = GetPropertyDefinition (typeof (Order), "Official");
      newDataContainer.SetValue (deliveryDateProperty, new DateTime (2005, 12, 24));
      newDataContainer.SetValue (customerProperty, DomainObjectIDs.Customer1);
      newDataContainer.SetValue (officialProperty, DomainObjectIDs.Official1);

      Provider.Save (new[] { newDataContainer });

      DataContainer loadedDataContainer = ReloadDataContainer (newDataContainer.ID);

      Assert.IsNotNull (loadedDataContainer);
      Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetValue (customerProperty));
      Assert.AreEqual (DomainObjectIDs.Official1, loadedDataContainer.GetValue (officialProperty));
    }

    [Test]
    public void NewRelatedObjects ()
    {
      var newCustomerDataContainer = CreateNewDataContainer (typeof (Customer));
      var newOrderDataContainer = CreateNewDataContainer (typeof (Order));

      var deliveryDateProperty = GetPropertyDefinition (typeof (Order), "DeliveryDate");
      var customerProperty = GetPropertyDefinition (typeof (Order), "Customer");
      newOrderDataContainer.SetValue (deliveryDateProperty, new DateTime (2005, 12, 24));
      newOrderDataContainer.SetValue (customerProperty, newCustomerDataContainer.ID);

      Provider.Save (new[] { newOrderDataContainer, newCustomerDataContainer });

      DataContainer reloadedCustomerContainer = ReloadDataContainer (newCustomerDataContainer.ID);
      DataContainer reloadedOrderContainer = ReloadDataContainer (newOrderDataContainer.ID);

      Assert.IsNotNull (reloadedCustomerContainer);
      Assert.IsNotNull (reloadedOrderContainer);
      Assert.AreEqual (reloadedCustomerContainer.ID, reloadedOrderContainer.GetValue (customerProperty));
    }

    [Test]
    public void SaveNullBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues (dataContainer);
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty");
      dataContainer.SetValue (propertyDefinition, null);

      Provider.Save (new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer (newID);
      Assert.IsNull (reloadedDataContainer.GetValue (propertyDefinition));
    }

    [Test]
    public void SaveEmptyBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues (dataContainer);
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty");
      dataContainer.SetValue (propertyDefinition, new byte[0]);

      Provider.Save (new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer (newID);
      ResourceManager.IsEmptyImage ((byte[]) reloadedDataContainer.GetValue (propertyDefinition));
    }

    [Test]
    public void SaveLargeBinary ()
    {
      DataContainer dataContainer = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = dataContainer.ID;

      SetDefaultValues (dataContainer);
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "BinaryProperty");
      dataContainer.SetValue (propertyDefinition, ResourceManager.GetImageLarger1MB ());

      Provider.Save (new[] { dataContainer });

      DataContainer reloadedDataContainer = ReloadDataContainer (newID);
      ResourceManager.IsEqualToImageLarger1MB ((byte[]) reloadedDataContainer.GetValue (propertyDefinition));
    }

    [Test]
    public void SaveEmpty ()
    {
      Provider.Save (new DataContainer[0]);
    }

    private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
    {
      // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
      SetPropertyValue (classWithAllDataTypesContainer, typeof (ClassWithAllDataTypes), "DateProperty", DateTime.Now);
      SetPropertyValue (classWithAllDataTypesContainer, typeof (ClassWithAllDataTypes), "DateTimeProperty", DateTime.Now);

      // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
      SetPropertyValue (classWithAllDataTypesContainer, typeof (ClassWithAllDataTypes), "DecimalProperty", 10m);
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

    private DataContainer CreateNewDataContainer (Type type)
    {
      return DataContainer.CreateNew (Provider.CreateNewObjectID (MappingConfiguration.Current.GetTypeDefinition (type)));
    }

  }
}