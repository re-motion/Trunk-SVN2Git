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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
  {
    [Test]
    public void Loading ()
    {
      DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer",
          DomainObjectIDs.Customer1);

      Assert.IsNotNull (collection);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
    }

    [Test]
    public void LoadOverInheritedProperty ()
    {
      DataContainer personContainer = Provider.LoadDataContainer (DomainObjectIDs.Person6);

      DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson",
          DomainObjectIDs.Person6);

      Assert.AreEqual (1, collection.Count);
      Assert.AreEqual (DomainObjectIDs.Distributor2, collection[0].ID);
    }

    [Test]
    public void LoadWithOrderBy ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (
          orderDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", DomainObjectIDs.Customer1);

      Assert.AreEqual (2, orderContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, orderContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orderContainers[1].ID);
    }

    [Test]
    public void LoadDataContainersByRelatedIDOfDifferentStorageProvider ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (orderDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official", DomainObjectIDs.Official1);
      Assert.IsNotNull (orderContainers);
      Assert.AreEqual (5, orderContainers.Count);
    }
  }
}
