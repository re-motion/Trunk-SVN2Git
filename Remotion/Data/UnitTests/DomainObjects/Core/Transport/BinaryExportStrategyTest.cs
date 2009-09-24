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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
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

      var items = new[] { item1, item2 };
      var versionIndependentItem1 = new KeyValuePair<string, Dictionary<string, object>> (item1.ID.ToString(), item1.Properties);
      var versionIndependentItem2 = new KeyValuePair<string, Dictionary<string, object>> (item2.ID.ToString(), item2.Properties);
      byte[] expectedData = Serializer.Serialize (new[] {versionIndependentItem1, versionIndependentItem2});

      byte[] actualData = Export (items);
      Assert.That (actualData, Is.EqualTo (expectedData));
    }

    public static byte[] Export (params TransportItem[] transportItems)
    {
      using (var stream = new MemoryStream ())
      {
        BinaryExportStrategy.Instance.Export (stream, transportItems);
        return stream.ToArray ();
      }
    }
  }
}
