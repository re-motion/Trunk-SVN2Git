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
      newDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"] = "123";

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

      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1974, 10, 25);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (
          1974, 10, 26, 18, 9, 18);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = (decimal) 564.956;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"] = 5334.2456;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] =
          ClassWithAllDataTypes.EnumType.Value0;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"] = Color.Values.Green();
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"] =
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"] = (short) 67;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"] = 42424242;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"] = 424242424242424242;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"] = (float) 42.42;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"] = "zyxwvuZaphodBeeblebrox";
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"] =
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876";
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage1();

      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 21;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (
          2005, 1, 18, 11, 11, 11);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 50m;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] =
          ClassWithAllDataTypes.EnumType.Value1;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] =
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}");
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) 51;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = 52;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = 53L;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = 54F;

      Provider.Save (new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer (newID);

      Assert.AreEqual (true, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual (42, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (
          new DateTime (1974, 10, 25), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (
          new DateTime (1974, 10, 26, 18, 9, 18),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (564.956, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (5334.2456d, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (
          Color.Values.Green(), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual (67, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (42424242, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (
          424242424242424242, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (42.42f, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (
          "zyxwvuZaphodBeeblebrox", reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876",
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEqualToImage1 (
          (byte[]) reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      Assert.AreEqual (false, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.AreEqual (21, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.AreEqual (
          new DateTime (2007, 1, 18), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 1, 18, 11, 11, 11),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.AreEqual (50m, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.AreEqual (56.87d, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value1,
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.AreEqual (51, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.AreEqual (52, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.AreEqual (53, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.AreEqual (54F, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
      Assert.IsNull (
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
    }

    [Test]
    public void AllDataTypes_DefaultValues ()
    {
      DataContainer classWithAllDataTypes = CreateNewDataContainer (typeof (ClassWithAllDataTypes));
      ObjectID newID = classWithAllDataTypes.ID;

      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1753, 1, 1);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (
          1753, 1, 1, 0, 0, 0);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = 0m;

      Provider.Save (new[] { classWithAllDataTypes });

      var reloadedClassWithAllDataTypes = ReloadDataContainer (newID);

      Assert.AreEqual (false, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual ((byte) 0, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (
          new DateTime (1753, 1, 1), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (
          new DateTime (1753, 1, 1, 0, 0, 0),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (0d, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (
          Color.Values.Blue(), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
      Assert.AreEqual (Guid.Empty, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual ((short) 0, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (0, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (0L, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (0F, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (string.Empty, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (
          string.Empty, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEmptyImage (
          (byte[]) reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
      Assert.IsNull (
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"]);
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
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
      Assert.AreEqual (newSupervisorContainer.ID, existingSubordinateContainer.GetValue (supervisorProperty, ValueAccess.Current));
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
      Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetValue (customerProperty, ValueAccess.Current));
      Assert.AreEqual (DomainObjectIDs.Official1, loadedDataContainer.GetValue (officialProperty, ValueAccess.Current));
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
      Assert.AreEqual (reloadedCustomerContainer.ID, reloadedOrderContainer.GetValue (customerProperty, ValueAccess.Current));
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
      Assert.IsNull (reloadedDataContainer.GetValue (propertyDefinition, ValueAccess.Current));
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
      ResourceManager.IsEmptyImage ((byte[]) reloadedDataContainer.GetValue (propertyDefinition, ValueAccess.Current));
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
      ResourceManager.IsEqualToImageLarger1MB ((byte[]) reloadedDataContainer.GetValue (propertyDefinition, ValueAccess.Current));
    }

    [Test]
    public void SaveEmpty ()
    {
      Provider.Save (new DataContainer[0]);
    }

    private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
    {
      // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = DateTime.Now;
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = DateTime.Now;

      // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = 10m;
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