using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
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
      Assert.AreEqual (0, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], "OrderNumber");
      Assert.AreEqual (DateTime.MinValue, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], "DeliveryDate");
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], "Official");
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], "Customer");
    }

    [Test]
    public void CreateClassWithAllDataTypes ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (classDefinition));

      Assert.AreEqual (false, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual ((byte)0, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (new DateTime (), newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (new DateTime (), newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (0m, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (0d, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (Guid.Empty, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual ((short)0, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (0, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (0L, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (0F, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEmptyImage ((byte[]) newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
      Assert.IsNull (newContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
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
