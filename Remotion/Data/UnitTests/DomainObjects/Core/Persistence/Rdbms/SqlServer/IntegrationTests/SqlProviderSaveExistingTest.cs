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
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] =
          ClassWithAllDataTypes.EnumType.Value2;

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value2,
            reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (false, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual (85, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 1, 1), savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 1, 1, 17, 0, 0),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (
          123456.789, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (
          987654.321, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value1,
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (
          Color.Values.Red(),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual (32767, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (2147483647, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (
          9223372036854775807, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (6789.321f, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (
          "abcdeföäü", savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (
          "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEqualToImage1 (
          (byte[]) savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1972, 10, 26);
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (
          1974, 10, 26, 15, 17, 19);
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = (decimal) 564.956;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"] = 5334.2456;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] =
          ClassWithAllDataTypes.EnumType.Value0;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"] =
          Color.Values.Green();
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"] =
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"] = (short) 67;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"] = 42424242;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"] = 424242424242424242;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"] = (float) 42.42;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"] = "zyxwvuZaphodBeeblebrox";
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"] =
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876";
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] =
          ResourceManager.GetImage2();

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual (42, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (
          new DateTime (1972, 10, 26),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (
          new DateTime (1974, 10, 26, 15, 17, 19),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (564.956, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (
          5334.2456, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (
          Color.Values.Green(),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual (67, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (42424242, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (
          424242424242424242, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (42.42f, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (
          "zyxwvuZaphodBeeblebrox",
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (
          "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876",
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEqualToImage2 (
          (byte[]) reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      DataContainer savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.AreEqual ((byte) 78, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 2, 1), savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 2, 1, 5, 0, 0),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.AreEqual (765.098m, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.AreEqual (
          654321.789d, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value2,
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.AreEqual (
          (short) 12000, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.AreEqual (-2147483647, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.AreEqual (3147483647L, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.AreEqual (12.456F, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
      Assert.IsNull (savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 100;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (
          2005, 1, 18, 10, 10, 10);
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 20.123m;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] =
          ClassWithAllDataTypes.EnumType.Value0;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] =
          new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}");
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) -43;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = -42;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = -41L;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = -40F;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] =
          ResourceManager.GetImage1();

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (false, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.AreEqual ((byte) 100, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.AreEqual (
          new DateTime (2007, 1, 18), reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 1, 18, 10, 10, 10),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.AreEqual (20.123m, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.AreEqual (56.87d, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value0,
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"),
          reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.AreEqual ((short) -43, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.AreEqual (-42, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.AreEqual (-41L, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.AreEqual (-40F, reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      DataContainer tempClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);
      tempClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] =
          ResourceManager.GetImage1();
      Provider.Save (new[] { tempClassWithAllDataTypes });
      
      var savedClassWithAllDataTypes = LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (true, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.AreEqual ((byte) 78, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 2, 1), savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.AreEqual (
          new DateTime (2005, 2, 1, 5, 0, 0),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.AreEqual (765.098m, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.AreEqual (
          654321.789d, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.AreEqual (
          ClassWithAllDataTypes.EnumType.Value2,
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.AreEqual (
          new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
          savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.AreEqual (
          (short) 12000, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.AreEqual (
          -2147483647, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.AreEqual (
          3147483647L, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.AreEqual (12.456F, savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
      ResourceManager.IsEqualToImage1 (
          (byte[]) savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = null;
      savedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;

      Provider.Save (new[] { savedClassWithAllDataTypes });

      DataContainer reloadedClassWithAllDataTypes = ReloadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

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
      Assert.IsNull (reloadedClassWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
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

      Assert.AreEqual (1, savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      Assert.AreEqual ("Mainboard", savedOrderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);

      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
      savedOrderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

      Provider.Save (new[] { savedOrderContainer, savedOrderItemContainer });

      DataContainer orderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderItemContainer = ReloadDataContainer (DomainObjectIDs.OrderItem1);

      Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      Assert.AreEqual ("Raumschiff", orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage =
        "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else."
        )]
    public void ConcurrentSave ()
    {
      DataContainer orderContainer1 = LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderContainer2 = LoadDataContainer (DomainObjectIDs.Order1);

      orderContainer1["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
      orderContainer2["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 11;

      Provider.Save (new[] { orderContainer1 });
      Provider.Save (new[] { orderContainer2 });
    }

    [Test]
    public void SaveWithConnect ()
    {
      DataContainer savedOrderContainer = LoadDataContainerWithSeparateProvider (DomainObjectIDs.Order1);
      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Assert.That (!Provider.IsConnected);

      Provider.Save (new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void WrapSqlException ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      PropertyDefinition newDefinition =
          MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem))["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"];

      orderContainer.PropertyValues.Add (new PropertyValue (newDefinition, "Raumschiff"));
      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Auto";

      Provider.Save (new[] { orderContainer });
    }

    [Test]
    public void UpdateTimestamps ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      object oldTimestamp = orderContainer.Timestamp;
      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { orderContainer });
      Provider.UpdateTimestamps (new[] { orderContainer });

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }

    [Test]
    public void UpdateTimestampsWithConnect ()
    {
      DataContainer orderContainer = LoadDataContainerWithSeparateProvider (DomainObjectIDs.Order1);
      object oldTimestamp = orderContainer.Timestamp;
      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

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

      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
      orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

      Provider.Save (new[] { orderContainer, orderItemContainer });
      Provider.UpdateTimestamps (new[] { orderContainer, orderItemContainer });

      Assert.IsFalse (oldOrderTimestamp.Equals (orderContainer.Timestamp));
      Assert.IsFalse (oldOrderItemTimestamp.Equals (orderItemContainer.Timestamp));
    }

    [Test]
    public void TransactionalSave ()
    {
      DataContainer savedOrderContainer = LoadDataContainer (DomainObjectIDs.Order1);

      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.BeginTransaction();
      Provider.Save (new[] { savedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      Provider.BeginTransaction ();
      DataContainer reloadedAndSavedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      reloadedAndSavedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { reloadedAndSavedOrderContainer });
      Provider.Commit();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
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

      orderTicketContainers[0]["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"] = "C:\newFile.jpg";

      Provider.Save (orderTicketContainers);
      Provider.Commit();

      DataContainer reloadedOrderTicketContainer = ReloadDataContainer (DomainObjectIDs.OrderTicket1);
      Assert.AreEqual ("C:\newFile.jpg", reloadedOrderTicketContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"]);
    }

    [Test]
    public void TransactionalSaveAndSetTimestamp ()
    {
      Provider.BeginTransaction();
      DataContainer savedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      object oldTimestamp = savedOrderContainer.Timestamp;
      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { savedOrderContainer });
      Provider.UpdateTimestamps (new[] { savedOrderContainer });
      Provider.Commit ();

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      Assert.IsFalse (oldTimestamp.Equals (reloadedOrderContainer.Timestamp));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Commit cannot be called without calling BeginTransaction first.")]
    public void CommitWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { orderContainer });
      Provider.Commit ();
    }

    [Test]
    public void SaveWithRollback ()
    {
      Provider.BeginTransaction ();
      DataContainer savedOrderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;

      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { savedOrderContainer });
      Provider.UpdateTimestamps (new[] { savedOrderContainer });
      Provider.Rollback ();

      DataContainer reloadedOrderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (1, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Rollback cannot be called without calling BeginTransaction first.")]
    public void RollbackWithoutBeginTransaction ()
    {
      DataContainer orderContainer = LoadDataContainer (DomainObjectIDs.Order1);
      orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      Provider.Save (new[] { orderContainer });
      Provider.Rollback ();
    }

    [Test]
    public void SaveForeignKeyInSameStorageProvider ()
    {
      DataContainer orderTicketContainer = LoadDataContainer (DomainObjectIDs.OrderTicket1);
      orderTicketContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"] = DomainObjectIDs.Order2;

      Provider.Save (new[] { orderTicketContainer });

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      DataContainer savedOrderContainer = LoadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      savedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"] = DomainObjectIDs.Official2;

      Provider.Save (new[] { savedOrderContainer });

      DataContainer reloadedOrderContainer = ReloadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      Assert.AreEqual (DomainObjectIDs.Official2, reloadedOrderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"]);
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer (DomainObjectIDs.Ceo1);
      savedCeoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"] = DomainObjectIDs.Partner1;

      Provider.Save (new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Partner1, reloadedCeoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      DataContainer savedCeoContainer = LoadDataContainer (DomainObjectIDs.Ceo1);
      savedCeoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"] = DomainObjectIDs.Supplier1;

      Provider.Save (new[] { savedCeoContainer });

      DataContainer reloadedCeoContainer = ReloadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Supplier1, reloadedCeoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
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
      savedCeoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"] = null;

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