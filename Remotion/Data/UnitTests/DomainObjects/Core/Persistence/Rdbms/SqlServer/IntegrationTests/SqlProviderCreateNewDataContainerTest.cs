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
  public class SqlProviderCreateNewDataContainerTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (orderClass));

      Assert.IsNotNull (newContainer, "New DataContainer is null.");
      Assert.IsNotNull (newContainer.ID, "ObjectID of new DataContainer.");
      Assert.AreEqual (orderClass.ID, newContainer.ID.ClassID, "ClassID of ObjectID.");
      Assert.AreEqual (c_testDomainProviderID, newContainer.ID.StorageProviderDefinition.Name, "StorageProviderID of ObjectID.");
      Assert.AreEqual (typeof (Guid), newContainer.ID.Value.GetType (), "Type of ID value of ObjectID.");
      Assert.IsNull (newContainer.Timestamp, "Timestamp of new DataContainer.");
      Assert.AreEqual (StateType.New, newContainer.State, "State of new DataContainer.");
      Assert.AreEqual (4, newContainer.PropertyValues.Count, "PropertyValues.Count");
      Assert.AreEqual (0, GetPropertyValue (newContainer, typeof (Order), "OrderNumber"), "OrderNumber");
      Assert.AreEqual (DateTime.MinValue, GetPropertyValue (newContainer, typeof (Order), "DeliveryDate"), "DeliveryDate");
      Assert.IsNull (GetPropertyValue (newContainer, typeof (Order), "Official"), "Official");
      Assert.IsNull (GetPropertyValue (newContainer, typeof (Order), "Customer"), "Customer");
    }

    [Test]
    public void CreateClassWithAllDataTypes ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes));
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (classDefinition));

      Assert.AreEqual (false, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "BooleanProperty"));
      Assert.AreEqual ((byte)0, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "ByteProperty"));
      Assert.AreEqual (new DateTime (), GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "DateProperty"));
      Assert.AreEqual (new DateTime (), GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "DateTimeProperty"));
      Assert.AreEqual (0m, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "DecimalProperty"));
      Assert.AreEqual (0d, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "DoubleProperty"));
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "EnumProperty"));
      Assert.AreEqual (Color.Values.Blue(), GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty"));
      Assert.AreEqual (Guid.Empty, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "GuidProperty"));
      Assert.AreEqual ((short)0, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "Int16Property"));
      Assert.AreEqual (0, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "Int32Property"));
      Assert.AreEqual (0L, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "Int64Property"));
      Assert.AreEqual (0F, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "SingleProperty"));
      Assert.AreEqual (string.Empty, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "StringProperty"));
      Assert.AreEqual (string.Empty, GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"));
      ResourceManager.IsEmptyImage ((byte[]) GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "BinaryProperty"));

      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaBooleanProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaByteProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDateProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDateTimeProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDecimalProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDoubleProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaEnumProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaGuidProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt16Property"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt32Property"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt64Property"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaSingleProperty"));

      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaBooleanWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaByteWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDateWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDecimalWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaDoubleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaGuidWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt16WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt32WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaInt64WithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NaSingleWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "StringWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty"));
      Assert.IsNull (GetPropertyValue (newContainer, typeof (ClassWithAllDataTypes), "NullableBinaryProperty"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ClassDefinition does not match with this StorageProvider's ID 'TestDomain'.\r\nParameter name: classDefinition")]
    public void ClassDefinitionOfOtherStorageProvider ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Official));
      Provider.CreateNewObjectID (classDefinition);
    }
  }
}
