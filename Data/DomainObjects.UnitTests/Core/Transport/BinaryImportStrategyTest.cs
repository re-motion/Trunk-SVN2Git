using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transport
{
  [TestFixture]
  public class BinaryImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      string orderNumberIdentifier = ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber");

      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Serialize(expectedContainer1, expectedContainer2);
      TransportItem[] items = EnumerableUtility.ToArray (BinaryImportStrategy.Instance.Import (data));
      Assert.AreEqual (2, items.Length);

      Assert.AreEqual (expectedContainer1.ID, items[0].ID);
      Assert.AreEqual (expectedContainer1.PropertyValues.Count, items[0].Properties.Count);
      Assert.AreEqual (expectedContainer1.PropertyValues[orderNumberIdentifier].Value, items[0].Properties[orderNumberIdentifier]);

      Assert.AreEqual (expectedContainer2.ID, items[1].ID);
      Assert.AreEqual (expectedContainer2.PropertyValues.Count, items[1].Properties.Count);
      Assert.AreEqual (expectedContainer2.PropertyValues[orderNumberIdentifier].Value, items[1].Properties[orderNumberIdentifier]);
    }

    private byte[] Serialize (params DataContainer[] containers)
    {
      TransportItem[] items = EnumerableUtility.ToArray (TransportItem.PackageDataContainers (containers));
      KeyValuePair<string, Dictionary<string, object>>[] versionIndependentItems =
          Array.ConvertAll<TransportItem, KeyValuePair<string, Dictionary<string, object>>> (
              items,
              delegate (TransportItem item) { return new KeyValuePair<string, Dictionary<string, object>>(item.ID.ToString(), item.Properties); });
      return Serializer.Serialize (versionIndependentItems);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Attempting to deserialize an empty stream.")]
    public void Import_ThrowsOnInvalidFormat ()
    {
      byte[] data = new byte[0];
      BinaryImportStrategy.Instance.Import (data);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Unable to cast object of type 'System.String' "
        + "to type 'System.Collections.Generic.KeyValuePair`2[System.String,System.Collections.Generic.Dictionary`2[System.String,System.Object]][]'.")]
    public void Import_ThrowsOnInvalidSerializedData ()
    {
      byte[] data = Serializer.Serialize ("string");
      BinaryImportStrategy.Instance.Import (data);
    }

    [Test]
    public void Import_ExportStrategy_IntegrationTest ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      item1.Properties.Add ("Foo", 12);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order2);
      item2.Properties.Add ("Bar", "42");

      byte[] package = BinaryExportStrategy.Instance.Export (new TransportItem[] { item1, item2 });
      TransportItem[] importedItems = EnumerableUtility.ToArray (BinaryImportStrategy.Instance.Import (package));

      Assert.AreEqual (2, importedItems.Length);
      Assert.AreEqual (item1.ID, importedItems[0].ID);
      Assert.AreEqual (1, importedItems[0].Properties.Count);
      Assert.AreEqual (item1.Properties["Foo"], importedItems[0].Properties["Foo"]);

      Assert.AreEqual (item2.ID, importedItems[1].ID);
      Assert.AreEqual (1, importedItems[1].Properties.Count);
      Assert.AreEqual (item2.Properties["Bar"], importedItems[1].Properties["Bar"]);
    }
  }
}