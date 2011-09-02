// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderLoadDataContainersTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadDataContainers_ByNonExistingID ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

      var result = Provider.LoadDataContainers (new[] { id }).ToList();

      Assert.That (result.Count, Is.EqualTo(1));
      Assert.That (result[0].LocatedObject, Is.Null);
      Assert.That (result[0].ObjectID, Is.SameAs(id));
    }

    [Test]
    public void LoadDataContainers_ByID ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      var actualContainer = Provider.LoadDataContainers (new[] { id }).ToArray()[0].LocatedObject;

      var expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypes1DataContainer ();

      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDataContainers_Multiple ()
    {
      var ids = new[] { DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order1, DomainObjectIDs.OrderItem2 };

      var containers = Provider.LoadDataContainers (ids).ToArray ();

      Assert.IsNotNull (containers);
      Assert.AreEqual (ids[0], containers[0].LocatedObject.ID);
      Assert.AreEqual (ids[1], containers[1].LocatedObject.ID);
      Assert.AreEqual (ids[2], containers[2].LocatedObject.ID);
      Assert.AreEqual (ids[3], containers[3].LocatedObject.ID);
    }
  }
}
