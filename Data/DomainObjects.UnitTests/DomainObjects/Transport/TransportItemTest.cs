/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Transport
{
  [TestFixture]
  public class TransportItemTest : ClientTransactionBaseTest
  {
    [Test]
    public void Initialization ()
    {
      TransportItem item = new TransportItem(DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, item.ID);
    }

    [Test]
    public void PackageDataContainer ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);

      CheckEqualData(container, item);
    }

    [Test]
    public void PackageDataContainers()
    {
      DataContainer container1 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      DataContainer container2 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem[] items = EnumerableUtility.ToArray (TransportItem.PackageDataContainers (new DataContainer[] { container1, container2 }));

      CheckEqualData (container1, items[0]);
      CheckEqualData (container2, items[1]);
    }

    public static void CheckEqualData (DataContainer expectedData, TransportItem item)
    {
      Assert.AreEqual (expectedData.ID, item.ID);
      Assert.AreEqual (expectedData.PropertyValues.Count, item.Properties.Count);
      foreach (PropertyValue propertyValue in expectedData.PropertyValues)
      {
        Assert.IsTrue (item.Properties.ContainsKey (propertyValue.Name));
        Assert.AreEqual (propertyValue.Value, item.Properties[propertyValue.Name]);
      }
    }

  }
}
