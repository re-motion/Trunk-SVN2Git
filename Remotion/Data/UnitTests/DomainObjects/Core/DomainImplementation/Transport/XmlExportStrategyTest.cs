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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation.Transport
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

      var items = new[] { item1, item2 };
      using (var stream = new MemoryStream ())
      {
        XmlExportStrategy.Instance.Export (stream, items);
        string actualString = Encoding.UTF8.GetString (stream.ToArray());
        Assert.That (actualString, Is.EqualTo (XmlSerializationStrings.XmlForOrder1Order2));
      }
    }

    public static byte[] Export (params TransportItem[] transportItems)
    {
      using (var stream = new MemoryStream ())
      {
        XmlExportStrategy.Instance.Export (stream, transportItems);
        return stream.ToArray ();
      }
    }
  }
}
