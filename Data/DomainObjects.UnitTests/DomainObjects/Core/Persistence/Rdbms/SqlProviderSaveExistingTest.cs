/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderSaveExistingTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    private DataContainer LoadDataContainer (SqlProvider sqlProvider, ObjectID id)
    {
      DataContainer dataContainer = sqlProvider.LoadDataContainer (id);
      ClientTransactionMock.SetClientTransaction (dataContainer);
      return dataContainer;
    }

    [Test]
    public void Save ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] = ClassWithAllDataTypes.EnumType.Value2;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      }
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (false, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (85, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (123456.789, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (987654.321, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (32767, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (2147483647, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (9223372036854775807, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (6789.321, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("abcdeföäü", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage1 ((byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1972, 10, 26);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (1974, 10, 26, 15, 17, 19);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = (decimal) 564.956;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"] = 5334.2456;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"] = ClassWithAllDataTypes.EnumType.Value0;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"] = new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"] = (short) 67;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"] = 42424242;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"] = 424242424242424242;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"] = (float) 42.42;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"] = "zyxwvuZaphodBeeblebrox";
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"] = "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876";
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage2 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (1972, 10, 26), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 26, 15, 17, 19), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (564.956, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (5334.2456, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (67, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (42424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (424242424242424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (42.42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage2 ((byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);
      }
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 78, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new DateTime (2005, 2, 1), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new DateTime (2005, 2, 1, 5, 0, 0), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (765.098m, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (654321.789d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual ((short) 12000, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (-2147483647, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (3147483647L, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (12.456F, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 100;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (2005, 1, 18, 10, 10, 10);
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 20.123m;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] = ClassWithAllDataTypes.EnumType.Value0;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}");
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) -43;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = -42;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = -41L;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = -40F;
        classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = ResourceManager.GetImage1 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (false, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 100, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new DateTime (2007, 1, 18), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 18, 10, 10, 10), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (20.123m, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (56.87d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual ((short) -43, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (-42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (-41L, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (-40F, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        ResourceManager.IsEqualToImage1 ((byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      DataContainer classWithAllDataTypes1;
      
      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        classWithAllDataTypes1 = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);
        classWithAllDataTypes1["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = ResourceManager.GetImage1 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes1);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes2 = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

        // need to make sure only one DomainObjectReference exists for the current ClientTransaction
        PrivateInvoke.InvokeNonPublicMethod (classWithAllDataTypes2, "SetDomainObject", classWithAllDataTypes1.DomainObject);

        Assert.AreEqual (true, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual ((byte) 78, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new DateTime (2005, 2, 1), classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new DateTime (2005, 2, 1, 5, 0, 0), classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (765.098m, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (654321.789d, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"), classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual ((short) 12000, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (-2147483647, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (3147483647L, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (12.456F, classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        ResourceManager.IsEqualToImage1 ((byte[]) classWithAllDataTypes2["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

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

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes2);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);

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
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = LoadDataContainer (sqlProvider, DomainObjectIDs.ClassWithAllDataTypes1);
        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        sqlProvider.Save (collection);
      }

      // expectation: no exception
    }


    [Test]
    public void SaveMultipleDataContainers ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        DataContainer orderItemContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.OrderItem1);

        Assert.AreEqual (1, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Mainboard", orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);
        collection.Add (orderItemContainer);

        sqlProvider.Save (collection);
      }


      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        DataContainer orderItemContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.OrderItem1);

        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Raumschiff", orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"]);
      }
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage = 
        "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else.")]
    public void ConcurrentSave ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer1 = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        DataContainer orderContainer2 = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        
        // need to make sure only one DomainObjectReference exists for the current ClientTransaction
        PrivateInvoke.InvokeNonPublicMethod (orderContainer2, "SetDomainObject", orderContainer1.DomainObject);

        orderContainer1["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderContainer2["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 11;

        DataContainerCollection collection1 = new DataContainerCollection ();
        collection1.Add (orderContainer1);
        sqlProvider.Save (collection1);

        DataContainerCollection collection2 = new DataContainerCollection ();
        collection2.Add (orderContainer2);
        sqlProvider.Save (collection2);
      }
    }

    [Test]
    public void SaveWithConnect ()
    {
      DataContainerCollection collection = new DataContainerCollection ();

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void WrapSqlException ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainerCollection collection = new DataContainerCollection ();
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        PropertyDefinition newDefinition =
            TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderItem)]["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"];

        orderContainer.PropertyValues.Add (new PropertyValue (newDefinition, "Raumschiff"));
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Auto";

        collection.Add (orderContainer);
        sqlProvider.Save (collection);
      }
    }

    [Test]
    public void SetTimestamp ()
    {
      object oldTimestamp = null;
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainerCollection collection = new DataContainerCollection ();
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.SetTimestamp (collection);

        Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
      }
    }

    [Test]
    public void SetTimestampWithConnect ()
    {
      DataContainerCollection collection = new DataContainerCollection ();
      DataContainer orderContainer = null;
      object oldTimestamp = null;

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.SetTimestamp (collection);
      }

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }

    [Test]
    public void SetTimestampForMultipleDataContainers ()
    {
      object oldOrderTimestamp = null;
      object oldOrderItemTimestamp = null;

      DataContainer orderContainer = null;
      DataContainer orderItemContainer = null;

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        orderItemContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.OrderItem1);

        oldOrderTimestamp = orderContainer.Timestamp;
        oldOrderItemTimestamp = orderItemContainer.Timestamp;

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product"] = "Raumschiff";

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);
        collection.Add (orderItemContainer);

        sqlProvider.Save (collection);
        sqlProvider.SetTimestamp (collection);
      }

      Assert.IsFalse (oldOrderTimestamp.Equals (orderContainer.Timestamp));
      Assert.IsFalse (oldOrderItemTimestamp.Equals (orderItemContainer.Timestamp));
    }

    [Test]
    public void TransactionalSave ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.BeginTransaction ();
        sqlProvider.Save (collection);
        sqlProvider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.BeginTransaction ();
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }


    [Test]
    public void TransactionalLoadDataContainersByRelatedIDAndSave ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.BeginTransaction ();

        DataContainerCollection orderTicketContainers = sqlProvider.LoadDataContainersByRelatedID (
            TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)],
            "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order",
            DomainObjectIDs.Order1);
        ClientTransactionMock.SetClientTransaction (orderTicketContainers[0]);

        orderTicketContainers[0]["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"] = "C:\newFile.jpg";

        sqlProvider.Save (orderTicketContainers);
        sqlProvider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderTicketContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.OrderTicket1);
        Assert.AreEqual ("C:\newFile.jpg", orderTicketContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"]);
      }
    }

    [Test]
    public void TransactionalSaveAndSetTimestamp ()
    {
      object oldTimestamp = null;
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.BeginTransaction ();
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        oldTimestamp = orderContainer.Timestamp;
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.SetTimestamp (collection);
        sqlProvider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
        Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Commit cannot be called without calling BeginTransaction first.")]
    public void CommitWithoutBeginTransaction ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.Commit ();
      }
    }

    [Test]
    public void SaveWithRollback ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.BeginTransaction ();
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);

        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.SetTimestamp (collection);
        sqlProvider.Rollback ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        Assert.AreEqual (1, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Rollback cannot be called without calling BeginTransaction first.")]
    public void RollbackWithoutBeginTransaction ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Order1);
        orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        sqlProvider.Save (collection);
        sqlProvider.Rollback ();
      }
    }

    [Test]
    public void SaveForeignKeyInSameStorageProvider ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        orderTicket.Order = Order.GetObject (DomainObjectIDs.Order2);

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (orderTicket.InternalDataContainer);

        sqlProvider.Save (collection);
      }

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        Order order = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
        order.Official = (Official) ClientTransactionMock.GetObject (DomainObjectIDs.Official2);

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (order.InternalDataContainer);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.OrderWithoutOrderItem);
        Assert.AreEqual (DomainObjectIDs.Official2, orderContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Partner.GetObject (DomainObjectIDs.Partner1);

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (ceo.InternalDataContainer);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer ceoContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Partner1, ceoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Supplier.GetObject (DomainObjectIDs.Supplier1);

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (ceo.InternalDataContainer);

        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer ceoContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Supplier1, ceoContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveNullForeignKey ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
        computer.Employee = null;

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (computer.InternalDataContainer);

        sqlProvider.Save (collection);
      }


      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer computerContainer = LoadDataContainer (sqlProvider, DomainObjectIDs.Computer1);
        Assert.IsNull (computerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));
      }
    }

    [Test]
    public void SaveNullForeignKeyWithInheritance ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = null;

        DataContainerCollection collection = new DataContainerCollection ();
				collection.Add (ceo.InternalDataContainer);

        sqlProvider.Save (collection);
      }

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

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot call BeginTransaction when a transaction is already in progress.")]
    public void CallBeginTransactionTwice ()
    {
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.BeginTransaction ();
        sqlProvider.BeginTransaction ();
      }
    }
  }
}
