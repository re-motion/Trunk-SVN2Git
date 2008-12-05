// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderSaveNewTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }


    private DataContainer CreateNewDataContainer (ClassDefinition classDefinition)
    {
      DataContainer dataContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (classDefinition));
      ClientTransactionMock.SetClientTransaction (dataContainer);
      return dataContainer;
    }

    private DataContainer LoadDataContainer (ObjectID id)
    {
      DataContainer dataContainer = Provider.LoadDataContainer (id);
      ClientTransactionMock.SetClientTransaction (dataContainer);
      return dataContainer;
    }

    [Test]
    public void NewDataContainer ()
    {
      DataContainer newDataContainer = CreateNewDataContainer(TestMappingConfiguration.Current.ClassDefinitions["Computer"]);

      newDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"] = "123";

      ObjectID newObjectID = newDataContainer.ID;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (newDataContainer);

      Provider.Save (collection);

      DataContainer loadedDataContainer = LoadDataContainer (newObjectID);

      Assert.IsNotNull (loadedDataContainer);
      Assert.AreEqual (newDataContainer.ID, loadedDataContainer.ID);
    }

    [Test]
    public void AllDataTypes ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];

      DataContainer classWithAllDataTypes = CreateNewDataContainer (classDefinition);
      ObjectID newID = classWithAllDataTypes.ID;

      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1974, 10, 25);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (1974, 10, 26, 18, 9, 18);
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
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage1 ();

      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 21;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (2005, 1, 18, 11, 11, 11);
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 50m;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] = ClassWithAllDataTypes.EnumType.Value1;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}");
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) 51;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = 52;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = 53L;
      classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = 54F;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        classWithAllDataTypes = LoadDataContainer(newID);

        Assert.AreEqual (true, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 25), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 26, 18, 9, 18), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (564.956, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (5334.2456d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (67, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (42424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (424242424242424242, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (42.42, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876", classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage1 ((byte[]) classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

        Assert.AreEqual (false, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (21, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new DateTime (2007, 1, 18), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 18, 11, 11, 11), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (50m, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (56.87d, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"), classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (51, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (52, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (53, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (54F, classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void ExistingObjectRelatesToNew ()
    {
      ClassDefinition employeeClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Employee)];
      Employee newSupervisor = Employee.NewObject ();
      Employee existingSubordinate = Employee.GetObject (DomainObjectIDs.Employee1);

      newSupervisor.Name = "Supervisor";
      existingSubordinate.Supervisor = newSupervisor;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (existingSubordinate.InternalDataContainer);
			collection.Add (newSupervisor.InternalDataContainer);

      Provider.Save (collection);

      DataContainer newSupervisorContainer = LoadDataContainer (newSupervisor.ID);
      DataContainer existingSubordinateContainer = LoadDataContainer (existingSubordinate.ID);

      Assert.IsNotNull (newSupervisorContainer);
      Assert.AreEqual (newSupervisorContainer.ID, existingSubordinateContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor"));
    }

    [Test]
    public void NewObjectRelatesToExisting ()
    {
      Order order = Order.NewObject ();
      order.DeliveryDate = new DateTime (2005, 12, 24);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);
      order.Official = Official.GetObject (DomainObjectIDs.Official1);

      ObjectID newObjectID = order.ID;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (order.InternalDataContainer);

      Provider.Save (collection);

      DataContainer loadedDataContainer = LoadDataContainer (newObjectID);

      Assert.IsNotNull (loadedDataContainer);
      Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
      Assert.AreEqual (DomainObjectIDs.Official1, loadedDataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"));
    }

    [Test]
    public void NewRelatedObjects ()
    {
      Customer newCustomer = Customer.NewObject ();
      Order newOrder = Order.NewObject ();
      Official existingOfficial = Official.GetObject (DomainObjectIDs.Official1);

      newOrder.DeliveryDate = new DateTime (2005, 12, 24);
      newOrder.Customer = newCustomer;
      newOrder.Official = existingOfficial;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (newOrder.InternalDataContainer);
			collection.Add (newCustomer.InternalDataContainer);

      Provider.Save (collection);

      DataContainer newCustomerContainer = LoadDataContainer (newCustomer.ID);
      DataContainer newOrderContainer = LoadDataContainer (newOrder.ID);

      Assert.IsNotNull (newCustomerContainer);
      Assert.IsNotNull (newOrderContainer);
      Assert.AreEqual (newCustomerContainer.ID, newOrderContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
      Assert.AreEqual (DomainObjectIDs.Official1, newOrderContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"));
    }

    [Test]
    public void SaveNullBinary ()
    {
      ObjectID newID;
      using (Provider)
      {
        DataContainer dataContainer = CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
        newID = dataContainer.ID;

        SetDefaultValues (dataContainer);
        dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        ClientTransactionMock.SetClientTransaction (dataContainer);
        Assert.IsNull (dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void SaveEmptyBinary ()
    {
      ObjectID newID;
      using (Provider)
      {
        DataContainer dataContainer = CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
        newID = dataContainer.ID;

        SetDefaultValues (dataContainer);
        dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = new byte[0];

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        ClientTransactionMock.SetClientTransaction (dataContainer);
        ResourceManager.IsEmptyImage ((byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
      }
    }

    [Test]
    public void SaveLargeBinary ()
    {
      ObjectID newID;
      using (Provider)
      {
        DataContainer dataContainer = CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
        newID = dataContainer.ID;

        SetDefaultValues (dataContainer);
        dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImageLarger1MB ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        ClientTransactionMock.SetClientTransaction (dataContainer);
        ResourceManager.IsEqualToImageLarger1MB ((byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      }
    }

    private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
    {
      // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"] = DateTime.Now;
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = DateTime.Now;

      // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
      classWithAllDataTypesContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = 10m;
    }
  }
}
