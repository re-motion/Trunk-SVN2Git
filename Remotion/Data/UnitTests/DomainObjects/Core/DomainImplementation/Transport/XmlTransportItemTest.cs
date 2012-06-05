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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation.Transport
{
  [TestFixture]
  public class XmlTransportItemTest : ClientTransactionBaseTest
  {
    [Test]
    public void Wrap ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order2);

      XmlTransportItem[] xmlItems = XmlTransportItem.Wrap (new TransportItem[] { item1, item2 });
      Assert.AreEqual (2, xmlItems.Length);
      Assert.AreEqual (item1, xmlItems[0].TransportItem);
      Assert.AreEqual (item2, xmlItems[1].TransportItem);
    }

    [Test]
    public void Unwrap ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order2);

      TransportItem[] items = XmlTransportItem.Unwrap (new XmlTransportItem[] { new XmlTransportItem  (item1), new XmlTransportItem (item2)});
      Assert.AreEqual (2, items.Length);
      Assert.AreEqual (item1, items[0]);
      Assert.AreEqual (item2, items[1]);
    }

    [Test]
    public void XmlSerialize ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForComputer1, serializedString);
    }

    [Test]
    public void XmlSerialize_WithNullObjectID ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer4).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForComputer4, serializedString);
    }

    [Test]
    public void XmlSerialize_WithCustomProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("Custom", 5);
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForCustomProperty, serializedString);
    }

    [Test]
    public void XmlSerialize_WithCustomObjectIDProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomReference", DomainObjectIDs.Order2);
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForCustomObjectIDProperty, serializedString);
    }

    [Test]
    public void XmlSerialize_WithCustomNullProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomNull", null);
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForCustomNullProperty, serializedString);
    }

    [Test]
    public void XmlSerialize_WithCustomExtensibleEnumProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomExtensibleEnum", Color.Values.Red());
      byte[] serializedArray = Serializer.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (XmlSerializationStrings.XmlForCustomExtensibleEnumProperty, serializedString);
    }

    [Test]
    public void XmlDeserialize ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForComputer1);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      TransportItemTest.CheckEqualData (Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer, item.TransportItem);
    }

    [Test]
    public void XmlDeserialize_WithNullObjectID ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForComputer4);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.IsNull (item.TransportItem.Properties[ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee")]);
    }

    [Test]
    public void XmlDeserialize_WithCustomProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomProperty);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.AreEqual (5, item.TransportItem.Properties["Custom"]);
    }

    [Test]
    public void XmlDeserialize_WithCustomObjectIDProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomObjectIDProperty);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.AreEqual (DomainObjectIDs.Order2, item.TransportItem.Properties["CustomReference"]);
    }

    [Test]
    public void XmlDeserialize_WithCustomNullProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomNullProperty);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.AreEqual (null, item.TransportItem.Properties["CustomNull"]);
    }

    [Test]
    public void XmlDeserialize_WithCustomExtensibleEnumProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomExtensibleEnumProperty);
      XmlTransportItem item = Serializer.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.AreEqual (Color.Values.Red (), item.TransportItem.Properties["CustomExtensibleEnum"]);
    }

    [Test]
    public void IntegrationTest_ID ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      Assert.AreEqual (container.ID, deserializedItem.ID);
    }

    [Test]
    public void IntegrationTest_Properties ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      TransportItemTest.CheckEqualData (container, deserializedItem);
    }

    [Test]
    public void IntegrationTest_Properties_IntVsString ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.ClassWithAllDataTypes1);
      item.Properties.Add ("Int", 1);
      item.Properties.Add ("String", "1");
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      Assert.AreEqual (1, deserializedItem.Properties["Int"]);
      Assert.AreEqual ("1", deserializedItem.Properties["String"]);
    }

    [Test]
    public void IntegrationTest_Multiple ()
    {
      DataContainer container1 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      DataContainer container2 = Computer.GetObject (DomainObjectIDs.Computer2).InternalDataContainer;
      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] deserializedItems = SerializeAndDeserialize (new TransportItem[] { item1, item2 });

      TransportItemTest.CheckEqualData (container1, deserializedItems[0]);
      TransportItemTest.CheckEqualData (container2, deserializedItems[1]);
    }

    [Test]
    public void IntegrationTest_ClassesWithAllDataTypes ()
    {
      DataContainer container1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).InternalDataContainer;
      DataContainer container2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2).InternalDataContainer;
      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] deserializedItems = SerializeAndDeserialize (new TransportItem[] { item1, item2 });

      TransportItemTest.CheckEqualData (container1, deserializedItems[0]);
      TransportItemTest.CheckEqualData (container2, deserializedItems[1]);
    }

    private TransportItem SerializeAndDeserialize (TransportItem item)
    {
      return Serializer.XmlSerializeAndDeserialize (new XmlTransportItem (item)).TransportItem;
    }

    private TransportItem[] SerializeAndDeserialize (TransportItem[] items)
    {
      return XmlTransportItem.Unwrap (Serializer.XmlSerializeAndDeserialize (XmlTransportItem.Wrap (items)));
    }
  }
}
