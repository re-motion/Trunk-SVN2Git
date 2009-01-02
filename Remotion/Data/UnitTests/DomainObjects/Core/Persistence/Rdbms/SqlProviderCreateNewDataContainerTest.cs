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
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderCreateNewDataContainerTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (orderClass));

      Assert.IsNotNull (newContainer, "New DataContainer is null.");
      Assert.IsNotNull (newContainer.ID, "ObjectID of new DataContainer.");
      Assert.AreEqual (orderClass.ID, newContainer.ID.ClassID, "ClassID of ObjectID.");
      Assert.AreEqual (c_testDomainProviderID, newContainer.ID.StorageProviderID, "StorageProviderID of ObjectID.");
      Assert.AreEqual (typeof (Guid), newContainer.ID.Value.GetType (), "Type of ID value of ObjectID.");
      Assert.IsNull (newContainer.Timestamp, "Timestamp of new DataContainer.");
      Assert.AreEqual (StateType.New, newContainer.State, "State of new DataContainer.");
      Assert.AreEqual (4, newContainer.PropertyValues.Count, "PropertyValues.Count");
      Assert.AreEqual (0, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"], "OrderNumber");
      Assert.AreEqual (DateTime.MinValue, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate"], "DeliveryDate");
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"], "Official");
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"], "Customer");
    }

    [Test]
    public void CreateClassWithAllDataTypes ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (classDefinition));

      Assert.AreEqual (false, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual ((byte)0, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (new DateTime (), newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (new DateTime (), newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (0m, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (0d, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (Guid.Empty, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual ((short)0, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (0, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (0L, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (0F, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEmptyImage ((byte[]) newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ClassDefinition does not match with this StorageProvider's ID 'TestDomain'.\r\nParameter name: classDefinition")]
    public void ClassDefinitionOfOtherStorageProvider ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Official)];
      Provider.CreateNewObjectID (classDefinition);
    }
  }
}
