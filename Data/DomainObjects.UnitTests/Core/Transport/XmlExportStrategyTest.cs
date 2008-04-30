using System;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transport
{
  [TestFixture]
  public class XmlExportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Export_SerializesData ()
    {
      DataContainer container1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer container2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] items = new TransportItem[] { item1, item2 };
      byte[] actualData = XmlExportStrategy.Instance.Export (items);
      string actualString = Encoding.UTF8.GetString (actualData);
      Assert.AreEqual (XmlSerializationStrings.XmlForOrder1Order2, actualString);
    }
  }
}