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
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation.Transport
{
  [TestFixture]
  public class XmlImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      var orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForOrder1Order2);

      TransportItem[] items = Import (data);
      Assert.AreEqual (2, items.Length);

      Assert.AreEqual (expectedContainer1.ID, items[0].ID);
      Assert.AreEqual (expectedContainer1.GetValue (orderNumberPropertyDefinition), items[0].Properties[orderNumberPropertyDefinition.PropertyName]);

      Assert.AreEqual (expectedContainer2.ID, items[1].ID);
      Assert.AreEqual (expectedContainer2.GetValue (orderNumberPropertyDefinition), items[1].Properties[orderNumberPropertyDefinition.PropertyName]);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: There is an error in XML document (0, 0).")]
    public void Import_ThrowsOnInvalidFormat ()
    {
      var data = new byte[0];
      Import (data);
    }

    [Test]
    public void Import_ExportStrategy_IntegrationTest ()
    {
      var item1 = new TransportItem (DomainObjectIDs.Order1);
      item1.Properties.Add ("Foo", 12);
      var item2 = new TransportItem (DomainObjectIDs.Order2);
      item2.Properties.Add ("Bar", "42");

      byte[] package = XmlExportStrategyTest.Export (item1, item2);
      TransportItem[] importedItems = Import (package);

      Assert.AreEqual (2, importedItems.Length);
      Assert.AreEqual (item1.ID, importedItems[0].ID);
      Assert.AreEqual (1, importedItems[0].Properties.Count);
      Assert.AreEqual (item1.Properties["Foo"], importedItems[0].Properties["Foo"]);

      Assert.AreEqual (item2.ID, importedItems[1].ID);
      Assert.AreEqual (1, importedItems[1].Properties.Count);
      Assert.AreEqual (item2.Properties["Bar"], importedItems[1].Properties["Bar"]);
    }

    public static TransportItem[] Import (byte[] data)
    {
      using (var stream = new MemoryStream (data))
      {
        return XmlImportStrategy.Instance.Import (stream).ToArray ();
      }
    }
  }
}
