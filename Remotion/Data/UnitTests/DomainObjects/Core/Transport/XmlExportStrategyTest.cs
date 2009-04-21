// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
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
