using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class BinaryExportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Export_SerializesData ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      TransportItem item1 = TransportItem.PackageDataContainer (expectedContainer1);
      TransportItem item2 = TransportItem.PackageDataContainer (expectedContainer2);

      TransportItem[] items = new TransportItem[] { item1, item2 };
      KeyValuePair<string, Dictionary<string, object>> versionIndependentItem1 =
          new KeyValuePair<string, Dictionary<string, object>> (item1.ID.ToString(), item1.Properties);
      KeyValuePair<string, Dictionary<string, object>> versionIndependentItem2 = 
          new KeyValuePair<string, Dictionary<string, object>> (item2.ID.ToString(), item2.Properties);

      byte[] expectedData = Serializer.Serialize (new KeyValuePair<string, Dictionary<string, object>>[] {versionIndependentItem1, versionIndependentItem2});
      byte[] actualData = BinaryExportStrategy.Instance.Export (items);
      Assert.That (actualData, Is.EqualTo (expectedData));
    }
  }
}