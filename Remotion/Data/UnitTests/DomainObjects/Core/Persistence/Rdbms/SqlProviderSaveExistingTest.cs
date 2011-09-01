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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderSaveExistingTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    private DataContainer LoadDataContainer (RdbmsProvider rdbmsProvider, ObjectID id)
    {
      DataContainer dataContainer = rdbmsProvider.LoadDataContainer (id).LocatedObject;
      return dataContainer;
    }

    [Test]
    public void Save ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value1,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] =
            ClassWithAllDataTypes.EnumType.Value2;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes);

        rdbmsProvider.Save (collection);
      }

      using (
          var rdbmsProvider = new RdbmsProvider (
              TestDomainStorageProviderDefinition,
              StorageNameProvider,
              SqlDialect.Instance,
              NullPersistenceListener.Instance,
              CommandFactory,
              ()=>new SqlConnection()))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value2,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      }
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (false, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (85, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 1, 1), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 1, 1, 17, 0, 0),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (123456.789, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (987654.321, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value1,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (
            Color.Values.Red(), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
        Assert.AreEqual (
            new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (32767, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (2147483647, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (
            9223372036854775807, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (6789.321f, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("abcdef���", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual (
            "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage1 (
            (byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1972, 10, 26);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (
            1974, 10, 26, 15, 17, 19);
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
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage2();

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (
            new DateTime (1972, 10, 26), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (
            new DateTime (1974, 10, 26, 15, 17, 19),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (564.956, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (5334.2456, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value0,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (
            Color.Values.Green(),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"]);
        Assert.AreEqual (
            new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (67, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (42424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (
            424242424242424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (42.42f, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual (
            "zyxwvuZaphodBeeblebrox", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual (
            "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876",
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage2 (
            (byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);
      }
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 78, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 2, 1), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 2, 1, 5, 0, 0),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (765.098m, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (
            654321.789d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value2,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (
            new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (
            (short) 12000, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (-2147483647, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (3147483647L, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (12.456F, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 100;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (
            2005, 1, 18, 10, 10, 10);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 20.123m;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] =
            ClassWithAllDataTypes.EnumType.Value0;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] =
            new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}");
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) -43;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = -42;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = -41L;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = -40F;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] =
            ResourceManager.GetImage1();

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (false, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 100, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (
            new DateTime (2007, 1, 18), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 1, 18, 10, 10, 10),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (20.123m, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (56.87d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value0,
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (
            new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"),
            classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual ((short) -43, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (-42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (-41L, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (-40F, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
      }
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      DataContainer classWithAllDataTypes1;

      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        classWithAllDataTypes1 = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);
        classWithAllDataTypes1["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] =
            ResourceManager.GetImage1();

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes1);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes2 = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 78, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 2, 1), classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (
            new DateTime (2005, 2, 1, 5, 0, 0),
            classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (765.098m, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (
            654321.789d, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (
            ClassWithAllDataTypes.EnumType.Value2,
            classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (
            new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"),
            classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (
            (short) 12000, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (
            -2147483647, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (
            3147483647L, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (12.456F, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        ResourceManager.IsEqualToImage1 (
            (byte[]) classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = null;
        classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes2);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void SaveWithNoChanges ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (rdbmsProvider, DomainObjectIDs.ClassWithAllDataTypes1);
        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (classWithAllDataTypes);

        rdbmsProvider.Save (collection);
      }

      // expectation: no exception
    }


    [Test]
    public void SaveMultipleDataContainers ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        DataContainer orderItemContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.OrderItem1);

        Assert.AreEqual (1, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Mainboard", orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);
        collection.Add (orderItemContainer);

        rdbmsProvider.Save (collection);
      }


      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        DataContainer orderItemContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.OrderItem1);

        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Raumschiff", orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);
      }
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage =
        "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else."
        )]
    public void ConcurrentSave ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer1 = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        DataContainer orderContainer2 = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        orderContainer1["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderContainer2["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 11;

        DataContainerCollection collection1 = new DataContainerCollection();
        collection1.Add (orderContainer1);
        rdbmsProvider.Save (collection1);

        DataContainerCollection collection2 = new DataContainerCollection();
        collection2.Add (orderContainer2);
        rdbmsProvider.Save (collection2);
      }
    }

    [Test]
    public void SaveWithConnect ()
    {
      DataContainerCollection collection = new DataContainerCollection();

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void WrapSqlException ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainerCollection collection = new DataContainerCollection();
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        PropertyDefinition newDefinition =
            MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem))["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"];

        orderContainer.PropertyValues.Add (new PropertyValue (newDefinition, "Raumschiff"));
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Auto";

        collection.Add (orderContainer);
        rdbmsProvider.Save (collection);
      }
    }

    [Test]
    public void UpdateTimestamps ()
    {
      object oldTimestamp = null;
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainerCollection collection = new DataContainerCollection();
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.UpdateTimestamps (collection);

        Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
      }
    }

    [Test]
    public void UpdateTimestampsWithConnect ()
    {
      DataContainerCollection collection = new DataContainerCollection();
      DataContainer orderContainer = null;
      object oldTimestamp = null;

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.UpdateTimestamps (collection);
      }

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }

    [Test]
    public void UpdateTimestampsForMultipleDataContainers ()
    {
      object oldOrderTimestamp = null;
      object oldOrderItemTimestamp = null;

      DataContainer orderContainer = null;
      DataContainer orderItemContainer = null;

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        orderItemContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.OrderItem1);

        oldOrderTimestamp = orderContainer.Timestamp;
        oldOrderItemTimestamp = orderItemContainer.Timestamp;

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);
        collection.Add (orderItemContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.UpdateTimestamps (collection);
      }

      Assert.IsFalse (oldOrderTimestamp.Equals (orderContainer.Timestamp));
      Assert.IsFalse (oldOrderItemTimestamp.Equals (orderItemContainer.Timestamp));
    }

    [Test]
    public void TransactionalSave ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.BeginTransaction();
        rdbmsProvider.Save (collection);
        rdbmsProvider.Commit();
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.BeginTransaction();
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.Commit();
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    public void TransactionalLoadDataContainersByRelatedIDAndSave ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.BeginTransaction();

        var relationEndPointDefinition = GetEndPointDefinition (typeof (OrderTicket), "Order");
        var orderTicketContainers = rdbmsProvider.LoadDataContainersByRelatedID (
            (RelationEndPointDefinition) relationEndPointDefinition,
            null,
            DomainObjectIDs.Order1).ToList();

        ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, orderTicketContainers[0]);

        orderTicketContainers[0]["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"] = "C:\newFile.jpg";

        rdbmsProvider.Save (orderTicketContainers);
        rdbmsProvider.Commit();
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderTicketContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.OrderTicket1);
        Assert.AreEqual ("C:\newFile.jpg", orderTicketContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"]);
      }
    }

    [Test]
    public void TransactionalSaveAndSetTimestamp ()
    {
      object oldTimestamp = null;
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.BeginTransaction();
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.UpdateTimestamps (collection);
        rdbmsProvider.Commit();
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Commit cannot be called without calling BeginTransaction first.")]
    public void CommitWithoutBeginTransaction ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.Commit();
      }
    }

    [Test]
    public void SaveWithRollback ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.BeginTransaction();
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.UpdateTimestamps (collection);
        rdbmsProvider.Rollback();
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (1, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Rollback cannot be called without calling BeginTransaction first.")]
    public void RollbackWithoutBeginTransaction ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderContainer);

        rdbmsProvider.Save (collection);
        rdbmsProvider.Rollback();
      }
    }

    [Test]
    public void SaveForeignKeyInSameStorageProvider ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        orderTicket.Order = Order.GetObject (DomainObjectIDs.Order2);

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (orderTicket.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        Order order = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
        order.Official = Official.GetObject (DomainObjectIDs.Official2);

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (order.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer orderContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.OrderWithoutOrderItem);
        Assert.AreEqual (DomainObjectIDs.Official2, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Partner.GetObject (DomainObjectIDs.Partner1);

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (ceo.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer ceoContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Partner1, ceoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Supplier.GetObject (DomainObjectIDs.Supplier1);

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (ceo.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }

      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer ceoContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Supplier1, ceoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveNullForeignKey ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
        computer.Employee = null;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (computer.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }


      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        DataContainer computerContainer = LoadDataContainer (rdbmsProvider, DomainObjectIDs.Computer1);
        Assert.IsNull (computerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));
      }
    }

    [Test]
    public void SaveNullForeignKeyWithInheritance ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = null;

        DataContainerCollection collection = new DataContainerCollection();
        collection.Add (ceo.InternalDataContainer);

        rdbmsProvider.Save (collection);
      }

      using (SqlConnection connection = new SqlConnection (TestDomainConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand ("select * from Ceo where ID = @id", connection))
        {
          command.Parameters.AddWithValue ("@id", DomainObjectIDs.Ceo1.Value);
          using (SqlDataReader reader = command.ExecuteReader())
          {
            reader.Read();
            int columnOrdinal = reader.GetOrdinal ("CompanyIDClassID");
            Assert.IsTrue (reader.IsDBNull (columnOrdinal));
          }
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot call BeginTransaction when a transaction is already in progress.")]
    public void CallBeginTransactionTwice ()
    {
      using (var rdbmsProvider = CreaterdbmsProvider())
      {
        rdbmsProvider.BeginTransaction();
        rdbmsProvider.BeginTransaction();
      }
    }

    private RdbmsProvider CreaterdbmsProvider ()
    {
      return new RdbmsProvider (
          TestDomainStorageProviderDefinition,
          StorageNameProvider,
          SqlDialect.Instance,
          NullPersistenceListener.Instance,
          CommandFactory,
          ()=>new SqlConnection());
    }
  }
}